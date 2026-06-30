using System;

namespace ITSupport.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public int CaseId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long? FileSize { get; set; }
        public string ContentType { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; }

        public Attachment() { }

        public Attachment(int attachmentId, int caseId, string fileName, string filePath,
                          long? fileSize, string contentType, int uploadedByUserId, DateTime uploadedAt)
        {
            AttachmentId = attachmentId;
            CaseId = caseId;
            FileName = fileName;
            FilePath = filePath;
            FileSize = fileSize;
            ContentType = contentType;
            UploadedByUserId = uploadedByUserId;
            UploadedAt = uploadedAt;
        }
    }
}
