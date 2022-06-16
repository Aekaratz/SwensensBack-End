namespace SwensenAPI.Models.Request
{
    public class RegisterRequest
    {
        public PersonData PersonData { get; set; }
        public PersonAddress PersonAddress { get; set; }
    }
    public class PersonData
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Conpassword { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class PersonAddress
    {
        public string Address { get; set; }
        public int TambonID { get; set; }
        public string Tambon { get; set; }
        public int DistrictID { get; set; }
        public string District { get; set; }
        public int CountyID { get; set; }
        public string County { get; set; }
        public int Postalcode { get; set; }

    }
}
