using System.Collections.Generic;

namespace UnitTestMocking
{
    public class InvoiceServiceDependencyInversion
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITaxService _taxService;
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceServiceDependencyInversion(ICustomerRepository customerRepository, ITaxService taxService, IInvoiceRepository invoiceRepository)
        {
            _customerRepository = customerRepository;
            _taxService = taxService;
            _invoiceRepository = invoiceRepository;
        }

        public bool CreateInvoice(Customer customer, IList<CartItem> cartItems)
        {
            if (!customer.Id.HasValue)
            {
                customer.Id = _customerRepository.CreateCustomer(customer);
            }

            var taxRate = _taxService.GetTaxRate(customer.Province);

            var invoice = new Invoice(customer.Id.Value);
            AddInvoiceItems(invoice, taxRate, cartItems);

            return _invoiceRepository.SaveInvoice(invoice);
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
}