using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace Pienty.Diariest.Core.Models.Database
{
    
    #region User
    
    [System.ComponentModel.DataAnnotations.Schema.Table("users")]
    public class User
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }

        public string user_hash { get; set; } = Guid.NewGuid().ToString();

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
    
    #endregion
    
    #region Agency

    [System.ComponentModel.DataAnnotations.Schema.Table("agency")]
    public class Agency
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        
        public bool active { get; set; }
        public bool deleted { get; set; }
        public Language language { get; set; }
        public DateTime created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public DateTime? deleted_date { get; set; }
    }
    
    [System.ComponentModel.DataAnnotations.Schema.Table("agency_users")]
    public class AgencyUser
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        public long agency_id { get; set; }
        public long user_id { get; set; }
        public bool active { get; set; }
        public bool deleted { get; set; }
        public DateTime created_date { get; set; }
        public DateTime? updated_date { get; set; }
        public DateTime? deleted_date { get; set; }
    }
    
    #endregion

    #region AppPage

    [System.ComponentModel.DataAnnotations.Schema.Table("app_pages")]
    public class AppPage
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long id { get; set; }
        
        [ForeignKey("id")]
        public long parent_id { get; set; }
        
        public string page_name { get; set; }
        public string endpoint { get; set; }
        
        
        [JsonIgnore]
        public bool deleted { get; set; }
        
        [JsonIgnore]
        public DateTime created_date { get; set; }

        [JsonIgnore]
        public DateTime? updated_date { get; set; }

        [JsonIgnore]
        public DateTime? deleted_date { get; set; }
        
        [Write(false)]
        public List<AppPage> Children { get; set; } = new List<AppPage>();
    }

    #endregion

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