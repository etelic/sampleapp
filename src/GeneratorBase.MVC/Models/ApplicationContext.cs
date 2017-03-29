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
			if (this.user != null && !this.user.IsAdmin)
			this.ApplyFilters(new List<IFilter<ApplicationContext>>()
                     {
                         new ApplicationSecurityFilter(user)                                         
                     });
        }	
        public IDbSet<FileDocument> FileDocuments { get; set; }
        public IDbSet<T_Client> T_Clients { get; set; }
        public IDbSet<T_LearningCenter> T_LearningCenters { get; set; }
        public IDbSet<T_Session> T_Sessions { get; set; }
        public IDbSet<T_TimeSlots> T_TimeSlotss { get; set; }
        public IDbSet<T_SessionEvents> T_SessionEventss { get; set; }
        public IDbSet<T_SessionClientAssociation> T_SessionClientAssociations { get; set; }
        public IDbSet<T_SessionEventsClient> T_SessionEventsClients { get; set; }
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
		public IDbSet<EntityDataSource> EntityDataSources { get; set; }
        public IDbSet<DataSourceParameters> DataSourceParameterss { get; set; }
        public IDbSet<PropertyMapping> PropertyMappings { get; set; }
		public IDbSet<T_Chart> Charts { get; set; }
		//Scheduler
		public IDbSet<T_Schedule> T_Schedules { get; set; }
        public IDbSet<T_Scheduletype> T_Scheduletypes { get; set; }
        public IDbSet<T_RecurringScheduleDetailstype> T_RecurringScheduleDetailstypes { get; set; }
        public IDbSet<T_RecurringFrequency> T_RecurringFrequencys { get; set; }
        public IDbSet<T_RecurringEndType> T_RecurringEndTypes { get; set; }
        public IDbSet<T_RecurrenceDays> T_RecurrenceDayss { get; set; }
        public IDbSet<T_MonthlyRepeatType> T_MonthlyRepeatTypes { get; set; }
        public IDbSet<T_RepeatOn> T_RepeatOns { get; set; }
		//Web Api
		public IDbSet<ApiToken> ApiTokens { get; set; }//will be used in case of webapi only
        //Custom Reports
        public IDbSet<CustomReport> CustomReports { get; set; }
        

		 
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
				if (!CheckIfModified(entry)) { CancelChanges(entry); continue; }
                if (ValidateBeforeSave(entry) == false)
                {
                    CancelChanges(entry); continue;
                }
                OnSaving(entry);
                //Add in list for business logic after successfully save.
                if (!originalStates.ContainsKey(entry))
                    originalStates.Add(entry, entry.State);
				if(CheckExternalAPISave(entry))
                {
                    CancelChanges(entry); continue;
                }
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
			TimeBasedAlert(entry);
            if (entry.State == EntityState.Modified)
            {
                AssignOneToManyCurrentOnUpdate(entry);
                MakeUpdateJournalEntry(entry);
            }
			OrderedListCheck(entry);
            if (entry.State == EntityState.Deleted)
                MakeDeleteJournalEntry(entry);
        }
        private void ApplyBusinessLogicAfterSave(Dictionary<DbEntityEntry, EntityState> originalStates)
        {
            if (originalStates != null)
            {
                if (originalStates.Any(p => p.Value.HasFlag(EntityState.Added)))
                {
					AssignOneToManyCurrentOnAdd(originalStates);
                    MakeAddJournalEntry(originalStates);
                    SetAutoProperty(originalStates);
                }
            }
            if (originalStates != null && originalStates.Any(p => p.Value.HasFlag(EntityState.Added) || p.Value.HasFlag(EntityState.Modified)))
            {
				InvokeActionRule(originalStates);
                EncryptValue(originalStates);
                TimeBasedAlert(originalStates);
                AfterSave(originalStates);
            }
        }
		private bool CheckExternalAPISave(DbEntityEntry entry)
        {
            var result = false;
            var entity = (IEntity)entry.Entity;
            var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
            var EntityName = entityType.Name;
            var state = entry.State;
            if (IsExternalEntity(entity,EntityName,state))
                result = true;
            return result;
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
			if (CheckLockCondition(entry))
                return false;
            if ((entry.State == EntityState.Added || entry.State == EntityState.Modified))
            {
                if (!Check1MThresholdCondition(entry))
                    return false;
				string strChkBeforeSave = CheckBeforeSave(entity, EntityName);
                if (!string.IsNullOrEmpty(strChkBeforeSave))
                {
                    throw new ArgumentException(strChkBeforeSave);
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

