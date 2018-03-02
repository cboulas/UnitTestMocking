using System;
using System.Collections.Generic;

namespace UnitTestMocking
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public class InvoiceService
    {
        public bool CreateInvoice(Customer customer, IList<CartItem> cartItems)
        {
            if (!customer.Id.HasValue)
            {
                var customerRepo = new CustomerRepository();
                customer.Id = customerRepo.CreateCustomer(customer);
            }

            var invoice = new Invoice(customer.Id.Value);

            var taxRate = GetTaxRate(customer.Province);

            foreach (var cartItem in cartItems)
            {
                var taxes = cartItem.Quantity * cartItem.Price * taxRate;

                var invoiceItem = new InvoiceItem
                {
                    Name = cartItem.Name,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price,
                    Taxes = taxes,
                    Total = (cartItem.Quantity * cartItem.Price) + taxes
                };
                invoice.Items.Add(invoiceItem);
            }

            var invoiceRepository = new InvoiceRepository();
            return invoiceRepository.SaveInvoice(invoice);
        }

        private decimal GetTaxRate(Province province)
        {
            switch (province)
            {
                case Province.Alberta:
                    return 0.05m;

                case Province.Saskatchewan:
                    return 0.11m;

                case Province.BritishColumbia:
                    return 0.12m;

                case Province.Ontario:
                case Province.Manitoba:
                    return 0.13m;

                case Province.Quebec:
                    return 0.14975m;

                case Province.NewBrunswick:
                case Province.NovaScotia:
                case Province.NewfoundlandLabrador:
                case Province.PrinceEdwardIsland:
                    return 0.15m;
                default:
                    throw new ArgumentOutOfRangeException(nameof(province), province, null);
            }
        }
        
    }

    public interface IInvoiceRepository
    {
        bool SaveInvoice(Invoice invoice);
    }

    public class InvoiceRepository
    {
        public bool SaveInvoice(Invoice invoice)
        {
            return true;
        }
    }

    public class Invoice
    {
        public Invoice(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentOutOfRangeException(nameof(customerId));

            CustomerId = customerId;
        }

        public int CustomerId { get; }
        public IList<InvoiceItem> Items { get; set; }

    }

    public class InvoiceItem : Item
    {
        public decimal Taxes { get; set; }
        public decimal Total { get; set; }
    }

    public class CartItem : Item
    {
    }

    public abstract class Item
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public interface ICustomerRepository
    {
        int CreateCustomer(Customer customer);
    }

    public class CustomerRepository
    {
        public int CreateCustomer(Customer customer)
        {
            return 1;
        }
    }

    public class Customer
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public Province Province { get; set; }

    }

    public enum Province
    {
        Quebec,
        Ontario,
        BritishColumbia,
        Alberta,
        Saskatchewan,
        Manitoba,
        NewBrunswick,
        NovaScotia,
        NewfoundlandLabrador,
        PrinceEdwardIsland
    }
}
