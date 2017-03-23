using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
namespace GeneratorBase.MVC
{
    public partial class ApplicationContext : DbContext
    {
        IUser user;
        public ApplicationContext(IUser user)
            : base("DefaultConnection")
        {
            this.user = user;
			//this.Database.Log = MvcApplication.LogToConsole;
			if (this.user != null && !this.user.IsAdmin())
			this.ApplyFilters(new List<IFilter<ApplicationContext>>()
                     {
                         new ApplicationSecurityFilter(user)                                         
                     });
        }	
        public IDbSet<FileDocument> FileDocuments { get; set; }
        public IDbSet<T_School> T_Schools { get; set; }
        public IDbSet<T_Student> T_Students { get; set; }
        public IDbSet<T_Department> T_Departments { get; set; }
        public IDbSet<T_StudentPerformance> T_StudentPerformances { get; set; }
		//Default DbSet for Application
		public IDbSet<Document> Documents { get; set; }
		public IDbSet<ImportConfiguration> ImportConfigurations { get; set; }     
		public IDbSet<FavoriteItem> FavoriteItems { get; set; }
		public IDbSet<DefaultEntityPage> DefaultEntityPages { get; set; }
		public IDbSet<DynamicRoleMapping> DynamicRoleMappings { get; set; }	 
		public IDbSet<ApplicationFeedback> ApplicationFeedbacks { get; set; }
		public IDbSet<ApplicationFeedbackType> ApplicationFeedbackTypes { get; set; }
		public IDbSet<ApplicationFeedbackStatus> ApplicationFeedbackStatuss { get; set; }
		public IDbSet<FeedbackPriority> FeedbackPrioritys { get; set; }
		public IDbSet<FeedbackSeverity> FeedbackSeveritys { get; set; }
		public IDbSet<FeedbackResource> FeedbackResources { get; set; }
		public IDbSet<AppSettingGroup> AppSettingGroups { get; set; }
		public IDbSet<AppSetting> AppSettings { get; set; }
		public IDbSet<EmailTemplateType> EmailTemplateTypes { get; set; }
		public IDbSet<EmailTemplate> EmailTemplates { get; set; }
				//End default DbSet for Application
        public override int SaveChanges()
        {
			var result = 0;
            var entries = this.ChangeTracker.Entries().Where(e => e.State.HasFlag(EntityState.Added) ||
                                                                   e.State.HasFlag(EntityState.Modified) ||
                                                                   e.State.HasFlag(EntityState.Deleted));
            var originalStates = new Dictionary<DbEntityEntry, EntityState>();
            foreach (var entry in entries)
            {
                if (ValidateBeforeSave(entry) == false)
                {
                    CancelChanges(entry); continue;
                }
                OnSaving(entry);
                //Add in list for business logic after successfully save.
                if (!originalStates.ContainsKey(entry))
                    originalStates.Add(entry, entry.State);
            }
            result = base.SaveChanges();//Save Changes
            ApplyBusinessLogicAfterSave(originalStates);
            return result;
        }
		private void OnSaving(DbEntityEntry entry)
        {
            if (entry.State != EntityState.Deleted)
                OnSavingCustom(entry, new ApplicationContext(user));
            else
                OnDeletingCustom(entry);
            SetCalculatedValue(entry);
            SetAutoProperty(entry);
            SetDisplayValue(entry);
            CheckFieldLevelSecurity(entry);
            if (entry.State == EntityState.Modified)
                MakeUpdateJournalEntry(entry);
            if (entry.State == EntityState.Deleted)
                MakeDeleteJournalEntry(entry);
        }
        private void ApplyBusinessLogicAfterSave(Dictionary<DbEntityEntry, EntityState> originalStates)
        {
            if (originalStates != null)
            {
                if (originalStates.Any(p => p.Value.HasFlag(EntityState.Added)))
                {
                    MakeAddJournalEntry(originalStates);
                    SetAutoProperty(originalStates);
                }
					BroadcastMessageForMonitoring(originalStates);
            }
            if (originalStates != null && originalStates.Any(p => p.Value.HasFlag(EntityState.Added) || p.Value.HasFlag(EntityState.Modified)))
            {
				InvokeActionRule(originalStates);
                EncryptValue(originalStates);
                TimeBasedAlert(originalStates);
                AfterSave(originalStates);
            }
        }
        private bool ValidateBeforeSave(DbEntityEntry entry)
        {
            var result = true;
            var entity = (IEntity)entry.Entity;
            var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
            var EntityName = entityType.Name;
            if (!CheckPermissionOnEntity(EntityName, entry.State))
                return false;
            if (CheckOwnerPermission(entry))
                return false;
            if (ViolatingBusinessRules(entry))
                return false;
            if ((entry.State == EntityState.Added || entry.State == EntityState.Modified))
            {
                if (!Check1MThresholdCondition(entry))
                    return false;
                if (!CheckBeforeSave(entity, EntityName))
                {
                    throw new ArgumentException("Validation Alert - Before Save !! Information not saved.");
                    return false;
                }
            }
            if (entry.State == EntityState.Deleted && !CheckBeforeDelete(entity, EntityName))
            {
                throw new ArgumentException("Validation Alert - Before Delete !! Record not deleted.");
                return false;
            }
            return result;
        }
    }
}

