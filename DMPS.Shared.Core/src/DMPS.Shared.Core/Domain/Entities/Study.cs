using DMPS.Shared.Core.Domain.Primitives;
using System;
using DMPS.Shared.Core.Common;

namespace DMPS.Shared.Core.Domain.Entities
{
    /// <summary>
    /// Represents a DICOM study, which is a collection of series of images for a single patient.
    /// This entity serves as an aggregate root for the DICOM data hierarchy.
    /// It supports features like 'working copies' for metadata editing (REQ-1-113) and data retention policies (REQ-1-086).
    /// </summary>
    public class Study : Entity<Guid>, IAggregateRoot
    {
        /// <summary>
        /// Gets the foreign key to the Patient record.
        /// </summary>
        public Guid PatientId { get; private set; }

        /// <summary>
        /// Gets the foreign key to the original study if this is a 'working copy'. Null otherwise.
        /// </summary>
        public Guid? OriginalStudyId { get; private set; }

        /// <summary>
        /// Gets the DICOM Study Instance UID (0020,000D).
        /// </summary>
        public string StudyInstanceUid { get; private set; }

        /// <summary>
        /// Gets the DICOM Study Date (0008,0020).
        /// </summary>
        public DateTime? StudyDate { get; private set; }

        /// <summary>
        /// Gets the DICOM Study Description (0008,1030).
        /// </summary>
        public string? StudyDescription { get; private set; }

        /// <summary>
        /// Gets the DICOM Accession Number (0008,0050).
        /// </summary>
        public string? AccessionNumber { get; private set; }

        /// <summary>
        /// Gets the DICOM Referring Physician's Name (0008,0090).
        /// </summary>
        public string? ReferringPhysicianName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this study has been soft-deleted as part of a data retention policy.
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Gets the timestamp when this study record was created in the system.
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Private constructor for EF Core.
        /// </summary>
        private Study() : base(Guid.NewGuid()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Study"/> class for a new study.
        /// </summary>
        /// <param name="id">The unique identifier for the study.</param>
        /// <param name="patientId">The identifier of the associated patient.</param>
        /// <param name="studyInstanceUid">The DICOM Study Instance UID.</param>
        /// <param name="studyDate">The DICOM Study Date.</param>
        /// <param name="studyDescription">The DICOM Study Description.</param>
        /// <param name="accessionNumber">The DICOM Accession Number.</param>
        /// <param name="referringPhysicianName">The DICOM Referring Physician's Name.</param>
        private Study(
            Guid id,
            Guid patientId,
            string studyInstanceUid,
            DateTime? studyDate,
            string? studyDescription,
            string? accessionNumber,
            string? referringPhysicianName) : base(id)
        {
            PatientId = Guard.Against.Default(patientId, nameof(patientId));
            StudyInstanceUid = Guard.Against.NullOrWhiteSpace(studyInstanceUid, nameof(studyInstanceUid));
            StudyDate = studyDate;
            StudyDescription = studyDescription;
            AccessionNumber = accessionNumber;
            ReferringPhysicianName = referringPhysicianName;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            OriginalStudyId = null;
        }

        /// <summary>
        /// Creates a new study record.
        /// </summary>
        public static Study Create(
            Guid patientId,
            string studyInstanceUid,
            DateTime? studyDate,
            string? studyDescription,
            string? accessionNumber,
            string? referringPhysicianName)
        {
            return new Study(
                Guid.NewGuid(),
                patientId,
                studyInstanceUid,
                studyDate,
                studyDescription,
                accessionNumber,
                referringPhysicianName);
        }

        /// <summary>
        /// Marks the study as deleted as part of a data purge or retention policy.
        /// </summary>
        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        /// <summary>
        /// Creates a working copy of the current study for metadata editing.
        /// </summary>
        /// <param name="newStudyInstanceUid">A new, unique Study Instance UID for the working copy.</param>
        /// <returns>A new Study instance representing the working copy.</returns>
        public Study CreateWorkingCopy(string newStudyInstanceUid)
        {
            var workingCopy = new Study(
                Guid.NewGuid(),
                this.PatientId,
                newStudyInstanceUid,
                this.StudyDate,
                this.StudyDescription,
                this.AccessionNumber,
                this.ReferringPhysicianName)
            {
                OriginalStudyId = this.Id
            };

            return workingCopy;
        }
        
        /// <summary>
        /// Updates the metadata of a study. This should typically be performed on a working copy.
        /// </summary>
        public void UpdateMetadata(
            string? studyDescription,
            string? accessionNumber,
            string? referringPhysicianName)
        {
            StudyDescription = studyDescription;
            AccessionNumber = accessionNumber;
            ReferringPhysicianName = referringPhysicianName;
        }
    }
}