using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public static string THEME_RETRO = "retro";
    public static string THEME_CARD = "card";
    public static string THEME_MAJONG = "majong";
    public static string THEME_SCI_FI = "scifi";
    public static string THEME_VISA = "visa";

    public static string THEME_LAWYER = "lawyer";
    public Button _button;

    void Awake()
    {
        if (PlayerPrefs.HasKey("LAWYER_TIME") == true)
        {
            _button.interactable = ((DateTime.Now.DayOfYear - PlayerPrefs.GetInt("LAWYER_TIME")) > 7) ? true : false;
        }

        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public static bool CheckBuyState(string id)
    {
        Product product = m_StoreController.products.WithID(id);
        if (product.hasReceipt) { return true; }
        else { return false; }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers. (РАсходуемые)
        builder.AddProduct(THEME_LAWYER, ProductType.Consumable);

        // Continue adding the non-consumable product. (Не расходуемые)
        builder.AddProduct(THEME_RETRO, ProductType.NonConsumable);
        builder.AddProduct(THEME_CARD, ProductType.NonConsumable);
        builder.AddProduct(THEME_MAJONG, ProductType.NonConsumable);
        builder.AddProduct(THEME_SCI_FI, ProductType.NonConsumable);
        builder.AddProduct(THEME_VISA, ProductType.NonConsumable);

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyConsumable(string name)
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(name);
    }


    public void BuyNonConsumable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(THEME_LAWYER);
    }


    public void BuySubscription()
    {
        // Buy the subscription product using its the general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.
    }


    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, THEME_RETRO, StringComparison.Ordinal))
        {
            CartController.thimeCart = 1;
            PlayerPrefs.SetInt("THEME_MONEY", CartController.thimeCart);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, THEME_CARD, StringComparison.Ordinal))
        {
            CartController.thimeCart = 2;
            PlayerPrefs.SetInt("THEME_MONEY", CartController.thimeCart);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, THEME_MAJONG, StringComparison.Ordinal))
        {
            CartController.thimeCart = 3;
            PlayerPrefs.SetInt("THEME_MONEY", CartController.thimeCart);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, THEME_SCI_FI, StringComparison.Ordinal))
        {
            CartController.thimeCart = 4;
            PlayerPrefs.SetInt("THEME_MONEY", CartController.thimeCart);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, THEME_VISA, StringComparison.Ordinal))
        {
            CartController.thimeCart = 5;
            PlayerPrefs.SetInt("THEME_MONEY", CartController.thimeCart);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, THEME_LAWYER, StringComparison.Ordinal))
        {
            PlayerPrefs.SetInt("LAWYER_TIME", DateTime.Now.DayOfYear);
            _button.interactable = false;
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
