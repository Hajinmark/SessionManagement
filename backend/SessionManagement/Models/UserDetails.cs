using System.ComponentModel.DataAnnotations;

namespace SessionManagement.Models
{
    public class UserDetails
    {
        [Key]
        public int UserDetailID { get; set; }
        // Foreign Key
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName {get ;set;}

        // Parent
        public Users Users { get; set; }

    }
}
