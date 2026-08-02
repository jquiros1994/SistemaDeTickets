using System.Collections.Generic;
using System.Text.RegularExpressions;
using ITSupport.DAL;
using ITSupport.Models;

namespace ITSupport.Business
{
    public class CustomerBusiness
    {
        private readonly CustomerRepository _customers;
        private readonly ProgramRepository _programs;

        public CustomerBusiness(CustomerRepository customers, ProgramRepository programs)
        {
            _customers = customers;
            _programs = programs;
        }

        public CustomerBusiness() : this(new CustomerRepository(), new ProgramRepository())
        {
        }

        public List<Customer> GetAll()
        {
            return _customers.GetAll();
        }

        public Customer GetById(int customerId)
        {
            return _customers.GetById(customerId);
        }

        public OperationResult Create(Customer model)
        {
            var errors = Validate(model);
            if (errors.Count > 0)
                return OperationResult.Fail(errors);

            if (_customers.GetByEmail(model.Email.Trim()) != null)
                return OperationResult.Fail("A customer with that email already exists.");

            model.IsActive = true;
            int newId = _customers.Insert(model);
            return OperationResult.Ok(newId);
        }

        private List<string> Validate(Customer model)
        {
            var errors = new List<string>();

            if (model == null)
            {
                errors.Add("No customer data received.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(model.FirstName))
                errors.Add("First name is required.");

            if (string.IsNullOrWhiteSpace(model.LastName))
                errors.Add("Last name is required.");

            if (string.IsNullOrWhiteSpace(model.Email) || !Regex.IsMatch(model.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("A valid email is required.");

            if (string.IsNullOrWhiteSpace(model.Country))
                errors.Add("Country is required.");

            if (model.ProgramId <= 0)
                errors.Add("You must select a program.");

            return errors;
        }

        public List<SupportProgram> GetPrograms()
        {
            return _programs.GetAll();
        }
    }
}
