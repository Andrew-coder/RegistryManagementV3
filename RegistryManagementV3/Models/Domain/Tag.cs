using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RegistryManagementV3.Models.Domain
{
    public sealed class Tag
    {
        public Tag()
        {
        }

        public Tag(string tagValue)
        {
            TagValue = tagValue;
        }

        public long Id { get; set; }

        [StringLength(30)]
        [Column(TypeName = "NVARCHAR(50)")]
        [Required]
        public string TagValue { get; set; }
        public ICollection<TagResources> Resources { get; set; }

        private bool Equals(Tag other)
        {
            return string.Equals(TagValue, other.TagValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tag) obj);
        }

        public override int GetHashCode()
        {
            return (TagValue != null ? TagValue.GetHashCode() : 0);
        }
    }
}