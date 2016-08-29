namespace PaymentsTU.Model
{
    public class Employee
    {
        public int Id { get; set; } 
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Note { get; set; }

        public string FullName
        {
            get
            {
                return string.Join(" ", new[] { Surname, Name, Patronymic });
            }
        }
    }
}