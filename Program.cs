using System;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.IO;
using System.Linq;

public class Product
{
    public string Name { get; set; }
    public bool Available { get; set; }
    public string Type { get; set; }

    public int Amount { get; set; }

    public Product(string name, bool available,string type, int amount)
    {
        Name = name;
        Available = available;
        Type = type;
        Amount = amount; 
    }
}


public class StockDataHandler
{
    public List<Product> LoadStockData()
    {
        try
        {
            if (File.Exists("stock_data.txt"))
            {
                return File.ReadAllLines("stock_data.txt")
                           .Select(line =>
                           {
                               var parts = line.Split(new[] { ", " }, StringSplitOptions.None);
                               if (parts.Length == 3)
                               {
                                   return new Product(parts[0], bool.Parse(parts[1]), parts[2], 0);        
                               }
                               return null;
                           })
                           .Where(product => product != null)
                           .ToList();
            }
            else
            {
                return new List<Product>
                {
                    new Product("Bananas", true, "food", 10 ),
                    new Product("Chocolate bars", true, "food", 20),
                    new Product("Pears", true, "food", 15),
                    new Product("Oranges", true, "food", 9),
                    new Product("Coconuts", true, "food", 7),
                    new Product("Coca-cola", true, "food", 13),
                    new Product("Coffee", true, "food", 5)   
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading stock data: {ex.Message}");
            return new List<Product>();
        }
    }

    public void SaveStockData(List<Product> stockMenu)
    {
        try
        {
            var lines = stockMenu.Select(product => $"{product.Name},{product.Available},{product.Type},{product.Amount}");     
            File.WriteAllLines("stock_data.txt", lines);
            Console.WriteLine("Stock data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving stock data: {ex.Message}");
        }
    }
}


public class StockManager
{
    private StockDataHandler dataHandler;
    private List<Product> stockMenu;

    public StockManager()
    {
        dataHandler = new StockDataHandler();
        stockMenu = dataHandler.LoadStockData();
    }

    public void Start()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("VISUAL STUDIO Stock List");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Choose from the options below");
            Console.WriteLine(" 1.Add\n 2.alter\n 3.Check\n 4.quit");
            string userInput = Console.ReadLine();
            var orderedStock = stockMenu.OrderBy(item => item.Name);

            if (userInput == "1")
            {
                Shopping();
            }
            else if (userInput == "2")
            {
                AlterStock();
                dataHandler.SaveStockData(stockMenu);
            }
            else if (userInput == "3")
            {
                CheckStock();
            }
            else if (userInput == "4")
            {
                dataHandler.SaveStockData(stockMenu);   
                return;
            }
            else if (userInput == "-v")
            {
                Console.WriteLine("Version control: 1.01");
            }
        }
    }

    private void CheckStock()
    {
        Console.WriteLine("What type of stock do you want to display: Food, Non-Food, or all?");
        string filter = Console.ReadLine()?.ToLower();

        IEnumerable<Product> filteredStock;

        switch (filter)
        {
            case "food":
                filteredStock = stockMenu.Where(product => string.Equals(product.Type, "food", StringComparison.OrdinalIgnoreCase));          
                break;
            case "non-food":
                filteredStock = stockMenu.Where(product => string.Equals(product.Type, "non-food", StringComparison.OrdinalIgnoreCase));
                break;
            case "all":
                filteredStock = stockMenu;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid product type: {filter}. Please enter 'food', 'non-food', or 'all'.");
                return;
        }

        Console.WriteLine($"Filter: {filter}, Count: {filteredStock.Count()}");

        if (filteredStock.Any())
        {
            foreach (var product in filteredStock)
            {
                Console.WriteLine($"{product.Name} - Type: {product.Type} - Amount: {product.Amount} - {(product.Available ? "Is Available" : "Isn't Available")}");
            }
        }
        else
        {
            Console.WriteLine($"No products found for the specified filter: {filter}");
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("------------------------------------------------------------------------------------------");
    }





    private void Shopping()
    {
        List<string> itemList = new List<string>();

        List<string> outOfStock = new List<string> { "milk", "sushi", "ice cream", "apples", "crisps", "falafel" };
        Console.WriteLine("Make a list to see whether we have any of the items in stock");
        Console.WriteLine("Add an item: ");

        while (true)
        {
            Console.WriteLine("Enter the item name (type 'quit' to exit): ");
            string itemName = Console.ReadLine();

            if (itemName.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("---------------------------------------------------------------------------");
                break;
            }

            Console.WriteLine("Is it a food product or non-food product? ");
            string productType = Console.ReadLine();

            Console.WriteLine("Enter the amount: ");

            if (int.TryParse(Console.ReadLine(), out int amount))
            {
                itemList.Add($"{itemName} - Type: {productType} - Amount: {amount}");
                stockMenu.Add(new Product(itemName, true, productType, amount));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Inavlid amount. Enter a valid number");
            }
        }

        dataHandler.SaveStockData(stockMenu);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Items not available:");
        foreach (var item in stockMenu)
        {
            if (!item.Available)
            {
                Console.WriteLine($"{item.Name} - Type: {item.Type} - Amount: {item.Amount}");
            }
        }
    }


    private void AlterStock()
    {
        string userinput = "start";
        while (userinput != "quit")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Select the option you want to pick");
            Console.WriteLine("\n1. Update\n2. Remove");
            userinput = Console.ReadLine();

            switch (userinput)
            {
                case "1":
                    Console.WriteLine("Enter the product name you want to update: ");
                    string productName = Console.ReadLine();
                    var existingProduct = stockMenu.Find(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));

                    if (existingProduct != null)
                    {
                        Console.WriteLine("Specify the type of product: ");
                        string updatedType = Console.ReadLine();

                        Console.WriteLine("Enter the new amount: ");
                        if (int.TryParse(Console.ReadLine(), out int updatedAmount))
                        {
                            existingProduct.Type = updatedType;
                            existingProduct.Amount = updatedAmount;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid amount. Please enter a valid number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Specify the type of product: ");
                        string addType = Console.ReadLine();

                        Console.WriteLine("Enter the amount: ");
                        if (int.TryParse(Console.ReadLine(), out int addAmount))
                        {
                            stockMenu.Add(new Product(productName, true, addType, addAmount));
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid amount. Please enter a valid number.");
                        }
                    }

                    dataHandler.SaveStockData(stockMenu);
                    break;

                case "2":
                    Console.WriteLine("What do you want to remove?: ");
                    string deleteItem = Console.ReadLine();
                    var productRemove = stockMenu.Where(p => p.Name.Equals(deleteItem, StringComparison.OrdinalIgnoreCase));

                    if (productRemove != null)
                    {
                        stockMenu.RemoveAll(p => p.Name.Equals(deleteItem, StringComparison.OrdinalIgnoreCase));
                        dataHandler.SaveStockData(stockMenu);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{deleteItem} not found in stock");
                    }
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The current stock is: ");
            foreach (var product in stockMenu)
            {
                Console.WriteLine(product.Name);
            }
        }
    }

    public static void Main()
    {
        StockManager stockManager = new StockManager();
        stockManager.Start();
    }
}
