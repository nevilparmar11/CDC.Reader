using System;

namespace CDC.Reader.Models
{
    public class EmployeeCT
    {
        public Guid GlobalEmployeeId { get; set; }

        public int EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime HiredDate { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public int OperationType { get; set; }

        public byte[] Start_LSN { get; set; }
    }
}
