using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pienty.Diariest.Core.Models.Database
{
    
    [Table("users")]
    public class User
    {
        [Key]
        public long id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        [JsonIgnore]
        public string password { get; set; }

        public string phone_number { get; set; }

        [JsonIgnore]
        public DateTime created_date { get; set; }

        [JsonIgnore]
        public DateTime? updated_date { get; set; }

        [JsonIgnore]
        public DateTime? deleted_date { get; set; }

        public bool active { get; set; }

        [JsonIgnore]
        public bool deleted { get; set; }

        public UserPermission permission { get; set; }

        public Language language { get; set; }
    }

    /*public class Website
    {
        [Key]
        public long Id { get; set; }
        
    }
    
    #region forms

    public class WebsiteForm
    {
        [Key]
        public long Id { get; set; }
        public long WebsiteId { get; set; }
    }
    
    #endregion
    
    #region tasks
    
    [Table("task_panel")]
    public class TaskPanel
    {
        [Key]
        public long Id { get; set; }
        public long CreatorId { get; set; }
            
        public string Name { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedDate { get; set; }
        [JsonIgnore]
        public DateTime? DeletedDate { get; set; }
        public bool Active { get; set; }
        [JsonIgnore]
        public bool Deleted { get; set; }
    }

    [Table("task_panel_members")]
    public class TaskPanelMember
    {
        public long UserId { get; set; }
        public long PanelId { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedDate { get; set; }
        [JsonIgnore]
        public DateTime? DeletedDate { get; set; }
    }
    
    #endregion*/
    
}