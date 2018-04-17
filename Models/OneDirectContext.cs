using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneDirect.Models
{
    public partial class OneDirectContext : DbContext
    {
        public virtual DbSet<ActivityLog> ActivityLog { get; set; }
        public virtual DbSet<AppointmentSchedule> AppointmentSchedule { get; set; }
        public virtual DbSet<Appointments> Appointments { get; set; }
        public virtual DbSet<Availability> Availability { get; set; }
        public virtual DbSet<DeviceCalibration> DeviceCalibration { get; set; }
        public virtual DbSet<EquipmentAssignment> EquipmentAssignment { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Pain> Pain { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientConfiguration> PatientConfiguration { get; set; }
        public virtual DbSet<PatientRx> PatientRx { get; set; }
        public virtual DbSet<Protocol> Protocol { get; set; }
        public virtual DbSet<RomchangeLog> RomchangeLog { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<TransactionLog> TransactionLog { get; set; }
        public virtual DbSet<User> User { get; set; }

        public OneDirectContext(DbContextOptions<OneDirectContext> options)
         : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //optionsBuilder.UseNpgsql(@"Host=ec2-107-20-249-48.compute-1.amazonaws.com;Port=5432;Database=ddqcspcd1k1sm9;User ID=twxbekgctwuybu;Password=23ca994792b75fca46f141750366bd59dbc34f319d97af96a3f3e5a3b508cbe6;sslmode=Require;Trust Server Certificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK_ActivityLog");

                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_fk_User_Patient");

                entity.HasIndex(e => e.UserId)
                    .HasName("fki_fk_User_ActivityLog");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.LinkToActivity)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.ActivityLog)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("fk_User_Patient");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ActivityLog)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_User_ActivityLog");
            });

            modelBuilder.Entity<AppointmentSchedule>(entity =>
            {
                entity.HasKey(e => e.AppointmentId)
                    .HasName("PK_AppointmentSchedule");

                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_fk_Patient_AppointmentSchedule");

                entity.HasIndex(e => e.UserId)
                    .HasName("fki_fk_User_AppointmentSchedule");

                entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");

                entity.Property(e => e.CallStatus).IsRequired();

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.RecordedFile).IsRequired();

                entity.Property(e => e.SlotStatus).IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserType).IsRequired();

                entity.Property(e => e.VseeUrl).HasColumnName("VseeURL");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.AppointmentSchedule)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("fk_Patient_AppointmentSchedule");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppointmentSchedule)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_User_AppointmentSchedule");
            });

            modelBuilder.Entity<Appointments>(entity =>
            {
                entity.HasKey(e => e.AppointmentId)
                    .HasName("PK_Appointments");

                entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");

                entity.Property(e => e.Duration).HasDefaultValueSql("0");

                entity.Property(e => e.PatientUserId).HasColumnName("PatientUserID");

                entity.Property(e => e.Status).HasDefaultValueSql("0");

                entity.Property(e => e.SupportUserId).HasColumnName("SupportUserID");

                entity.Property(e => e.Timezone).HasColumnName("timezone");

                entity.Property(e => e.Urikey).HasColumnName("URIKey");
            });

            modelBuilder.Entity<Availability>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("fki_fk_User_Availability");

                entity.Property(e => e.AvailabilityId).HasColumnName("AvailabilityID");

                entity.Property(e => e.DayOfWeek).IsRequired();

                entity.Property(e => e.HourOfDay).IsRequired();

                entity.Property(e => e.TimeZoneOffset).IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserType).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Availability)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_User_Availability");
            });

            modelBuilder.Entity<DeviceCalibration>(entity =>
            {
                entity.HasKey(e => e.SetupId)
                    .HasName("PK_DeviceCalibration");

                entity.Property(e => e.SetupId).HasColumnName("SetupID");

                entity.Property(e => e.Actuator1ExtendedAngle).HasColumnName("Actuator1_Extended_Angle");

                entity.Property(e => e.Actuator1ExtendedPulse).HasColumnName("Actuator1_Extended_Pulse");

                entity.Property(e => e.Actuator1NeutralAngle).HasColumnName("Actuator1_Neutral_Angle");

                entity.Property(e => e.Actuator1NeutralPulse).HasColumnName("Actuator1_Neutral_Pulse");

                entity.Property(e => e.Actuator1RetractedAngle).HasColumnName("Actuator1_Retracted_Angle");

                entity.Property(e => e.Actuator1RetractedPulse).HasColumnName("Actuator1_Retracted_Pulse");

                entity.Property(e => e.Actuator2ExtendedAngle).HasColumnName("Actuator2_Extended_Angle");

                entity.Property(e => e.Actuator2ExtendedPulse).HasColumnName("Actuator2_Extended_Pulse");

                entity.Property(e => e.Actuator2NeutralAngle).HasColumnName("Actuator2_Neutral_Angle");

                entity.Property(e => e.Actuator2NeutralPulse).HasColumnName("Actuator2_Neutral_Pulse");

                entity.Property(e => e.Actuator2RetractedAngle).HasColumnName("Actuator2_Retracted_Angle");

                entity.Property(e => e.Actuator2RetractedPulse).HasColumnName("Actuator2_Retracted_Pulse");

                entity.Property(e => e.Actuator3ExtendedAngle).HasColumnName("Actuator3_Extended_Angle");

                entity.Property(e => e.Actuator3ExtendedPulse).HasColumnName("Actuator3_Extended_Pulse");

                entity.Property(e => e.Actuator3NeutralAngle).HasColumnName("Actuator3_Neutral_Angle");

                entity.Property(e => e.Actuator3NeutralPulse).HasColumnName("Actuator3_Neutral_Pulse");

                entity.Property(e => e.Actuator3RetractedAngle).HasColumnName("Actuator3_Retracted_Angle");

                entity.Property(e => e.Actuator3RetractedPulse).HasColumnName("Actuator3_Retracted_Pulse");

                entity.Property(e => e.BoomId1)
                    .IsRequired()
                    .HasColumnName("BoomID1");

                entity.Property(e => e.BoomId2).HasColumnName("BoomID2");

                entity.Property(e => e.BoomId3).HasColumnName("BoomID3");

                entity.Property(e => e.ChairId)
                    .IsRequired()
                    .HasColumnName("ChairID");

                entity.Property(e => e.ControllerId)
                    .IsRequired()
                    .HasColumnName("ControllerID");

                entity.Property(e => e.DeviceConfiguration).IsRequired();

                entity.Property(e => e.EquipmentType).IsRequired();

                entity.Property(e => e.InstallerId)
                    .IsRequired()
                    .HasColumnName("InstallerID");

                entity.Property(e => e.MacAddress).IsRequired();

                entity.Property(e => e.NewControllerId).HasColumnName("NewControllerID");

                entity.Property(e => e.PatientSide).IsRequired();

                entity.Property(e => e.TabletId)
                    .IsRequired()
                    .HasColumnName("TabletID")
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.UpdatePending).IsRequired();
            });

            modelBuilder.Entity<EquipmentAssignment>(entity =>
            {
                entity.HasKey(e => e.AssignmentId)
                    .HasName("PK_EquipmentAssignment");

                entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");

                entity.Property(e => e.Boom1Id).HasColumnName("Boom1ID");

                entity.Property(e => e.Boom2Id).HasColumnName("Boom2ID");

                entity.Property(e => e.Boom3Id).HasColumnName("Boom3ID");

                entity.Property(e => e.ChairId).HasColumnName("ChairID");

                entity.Property(e => e.DateInstalled).HasColumnType("date");

                entity.Property(e => e.DateRemoved).HasColumnType("date");

                entity.Property(e => e.InstallerId)
                    .IsRequired()
                    .HasColumnName("InstallerID");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.HasOne(d => d.Installer)
                    .WithMany(p => p.EquipmentAssignment)
                    .HasForeignKey(d => d.InstallerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fx_therapist_user");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.EquipmentAssignment)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fx_patient_user");
            });

            modelBuilder.Entity<Messages>(entity =>
            {
                entity.HasIndex(e => e.ReceiverId)
                    .HasName("fki_fx_pain_userId_receiver");

                entity.HasIndex(e => e.SenderId)
                    .HasName("fki_fx_pain_user");

                entity.Property(e => e.MessageId)
                    .IsRequired()
                    .HasColumnName("MessageID");

                entity.Property(e => e.ReadStatus).HasDefaultValueSql("0");

                entity.Property(e => e.ReceiverId)
                    .IsRequired()
                    .HasColumnName("ReceiverID");

                entity.Property(e => e.SenderId)
                    .IsRequired()
                    .HasColumnName("SenderID");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.MessagesReceiver)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fx_pain_userId_receiver");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.MessagesSender)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fx_pain_user");
            });

            modelBuilder.Entity<Pain>(entity =>
            {
                entity.Property(e => e.ProtocolId)
                    .IsRequired()
                    .HasColumnName("ProtocolID");

                entity.Property(e => e.RxId).HasColumnName("RxID");

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasColumnName("SessionID");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Pain)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_Session_Pain");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.PatientLoginId)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.PatientName).IsRequired();

                entity.Property(e => e.Pin).HasColumnName("PIN");

                entity.Property(e => e.ProviderId)
                    .IsRequired()
                    .HasColumnName("ProviderID");

                entity.Property(e => e.Ssn).HasColumnName("SSN");

                entity.Property(e => e.Therapistid).HasColumnName("therapistid");
            });

            modelBuilder.Entity<PatientConfiguration>(entity =>
            {
                entity.HasIndex(e => e.InstallerId)
                    .HasName("fki_fx_User_PatientConfiguration");

                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_fk_Patient_PatientConfiguration");

                entity.HasIndex(e => e.SetupId)
                    .HasName("fki_fk_DeviceCalibration_PatientConfiguration");

                entity.Property(e => e.DeviceConfiguration).IsRequired();

                entity.Property(e => e.EquipmentType).IsRequired();

                entity.Property(e => e.InstallerId)
                    .IsRequired()
                    .HasColumnName("InstallerID")
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.PatientFirstName)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.PatientSide).IsRequired();

                entity.Property(e => e.RxId)
                    .IsRequired()
                    .HasColumnName("RxID");

                entity.Property(e => e.SetupId)
                    .IsRequired()
                    .HasColumnName("SetupID");

                entity.HasOne(d => d.Installer)
                    .WithMany(p => p.PatientConfiguration)
                    .HasForeignKey(d => d.InstallerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fx_User_PatientConfiguration");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientConfiguration)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_Patient_PatientConfiguration");

                entity.HasOne(d => d.Setup)
                    .WithMany(p => p.PatientConfiguration)
                    .HasForeignKey(d => d.SetupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_DeviceCalibration_PatientConfiguration");
            });

            modelBuilder.Entity<PatientRx>(entity =>
            {
                entity.HasKey(e => e.RxId)
                    .HasName("PK_PatientRx");

                entity.Property(e => e.RxId).HasColumnName("RxID");

                entity.Property(e => e.CurrentExtension).HasDefaultValueSql("0");

                entity.Property(e => e.CurrentFlexion).HasDefaultValueSql("0");

                entity.Property(e => e.DeviceConfiguration)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.GoalExtension).HasDefaultValueSql("0");

                entity.Property(e => e.GoalFlexion).HasDefaultValueSql("0");

                entity.Property(e => e.PainThreshold).HasDefaultValueSql("0");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

                entity.Property(e => e.RateOfChange).HasDefaultValueSql("0");

                entity.Property(e => e.RxDaysPerweek).HasDefaultValueSql("0");

                entity.Property(e => e.RxSessionsPerWeek).HasDefaultValueSql("0");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientRx)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_Patient_Patientrx");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.PatientRx)
                    .HasForeignKey(d => d.ProviderId)
                    .HasConstraintName("fk_User_PatientRx");
            });

            modelBuilder.Entity<Protocol>(entity =>
            {
                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_Patient_Protocol");

                entity.Property(e => e.ProtocolId).HasColumnName("ProtocolID");

                entity.Property(e => e.DeviceConfiguration)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.DownReps).HasDefaultValueSql("0");

                entity.Property(e => e.EquipmentType)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.FlexUpHoldtime).HasDefaultValueSql("0");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                entity.Property(e => e.ProtocolEnum).HasDefaultValueSql("0");

                entity.Property(e => e.ProtocolName).IsRequired();

                entity.Property(e => e.Reps).HasDefaultValueSql("0");

                entity.Property(e => e.RepsAt).HasDefaultValueSql("0");

                entity.Property(e => e.RestAt).HasDefaultValueSql("0");

                entity.Property(e => e.RestPosition).HasDefaultValueSql("0");

                entity.Property(e => e.RestTime).HasDefaultValueSql("0");

                entity.Property(e => e.RxId)
                    .IsRequired()
                    .HasColumnName("RxID");

                entity.Property(e => e.Speed).HasDefaultValueSql("0");

                entity.Property(e => e.StretchUpHoldtime).HasDefaultValueSql("0");

                entity.Property(e => e.UpReps).HasDefaultValueSql("0");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Protocol)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_Patient_Protocol");

                entity.HasOne(d => d.Rx)
                    .WithMany(p => p.Protocol)
                    .HasForeignKey(d => d.RxId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_Patientrx_Protocol");
            });

            modelBuilder.Entity<RomchangeLog>(entity =>
            {
                entity.ToTable("ROMChangeLog");

                entity.Property(e => e.ChangedBy).IsRequired();

                entity.Property(e => e.RxId)
                    .IsRequired()
                    .HasColumnName("RxID");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasIndex(e => e.PatientId)
                    .HasName("fki_Patient_Session");

                entity.Property(e => e.SessionId).HasColumnName("SessionID");

                entity.Property(e => e.ProtocolId)
                    .IsRequired()
                    .HasColumnName("ProtocolID");

                entity.Property(e => e.RxId)
                    .IsRequired()
                    .HasColumnName("RxID");

                entity.Property(e => e.TimeZoneOffset)
                    .IsRequired()
                    .HasDefaultValueSql("''::text");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_Patient_Session");

                entity.HasOne(d => d.Protocol)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.ProtocolId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_Protocol_Session");

                entity.HasOne(d => d.Rx)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.RxId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_PatientRx_Session");
            });

            modelBuilder.Entity<TransactionLog>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK_TransactionLog");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.Property(e => e.Duration).HasDefaultValueSql("0");

                entity.Property(e => e.PatientUserId).HasColumnName("PatientUserID");

                entity.Property(e => e.TransactionType).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Npi).HasColumnName("NPI");

                entity.Property(e => e.Vseeid).HasColumnName("vseeid");
            });
        }
    }
}