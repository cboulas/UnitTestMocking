using System;
using System.Collections.Generic;

namespace UnitTestMocking
{
    public class InvoiceServiceSingleResponsibility
    {
        public bool CreateInvoice(Customer customer, IList<CartItem> cartItems)
        {
            if (!customer.Id.HasValue)
            {
                var customerRepo = new CustomerRepository();
                customer.Id = customerRepo.CreateCustomer(customer);
            }

            var taxService = new TaxService();
            var taxRate = taxService.GetTaxRate(customer.Province);

            var invoice = new Invoice(customer.Id.Value);
            AddInvoiceItems(invoice, taxRate, cartItems);

            var invoiceRepository = new InvoiceRepository();
            return invoiceRepository.SaveInvoice(invoice);
        }

        private void AddInvoiceItems(Invoice invoice, decimal taxRate, IList<CartItem> cartItems)
        {
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
        }
        
    }

    public interface ITaxService
    {
        decimal GetTaxRate(Province province);
    }

    public class TaxService
    {
        public decimal GetTaxRate(Province province)
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
}