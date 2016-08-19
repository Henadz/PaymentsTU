using System;
using System.Collections.Generic;

namespace PaymentsTU.Model
{
    internal sealed class Dal
    {
        public static IList<FinancialPeriod> FinancialPeriods()
        {
            return new[]
            {
                new FinancialPeriod
                {
                    Id = 0,
                    StartDate = new DateTime(2016, 1, 1),
                    EndDate = new DateTime(2016, 12, 31),
                    IsClosed = false,
                    PaymentLimit = (decimal) 260.00
                }
            };
        }

        public bool ClosePeriod(int id)
        {
            return id >= 0;
        }

        public int UpdatePeriod(FinancialPeriod period)
        {
            return period.Id.HasValue ? 1 : 0;
        }

        public PaymentMatrix PaymentsInformation(DateTime startDate, DateTime endDate)
        {
            return new PaymentMatrix();
        }

        public IList<Employee> Employees()
        {
            return new[]
            {
                new Employee
                {
                    Id = 0,
                    Surname = "Linevich",
                    Name = "Viktoria",
                    Patronymic = "Vladimirovna",
                    Note = "Cheaf"
                },
                new Employee
                {
                    Id = 1,
                    Surname = "Linevich",
                    Name = "Henadz",
                    Patronymic = "Anatolievich",
                    Note = "Man"
                }
            };
        }

        public IList<PaymentType> PaymentTypes()
        {
            return new[]
            {
                new PaymentType
                {
                    Id = 0,
                    Name = "Viktoria"
                },
                new PaymentType
                {
                    Id = 1,
                    Name = "Henadz"
                }
            };
        } 

    }
}