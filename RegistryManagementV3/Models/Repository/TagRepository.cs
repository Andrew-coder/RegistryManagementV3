using System.Linq;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Models.Repository
{
    public class TagRepository : Repository<Tag>
    {
        public TagRepository(SecurityDbContext context) : base(context)
        {
        }

        public Tag FindTagByText(string text)
        {
            return Context.Tags.FirstOrDefault(tag => tag.TagValue.Equals(text));
        }

        public bool CheckIfExists(string text)
        {
            return Context.Tags.Any(tag => tag.TagValue.Equals(text));
        }
    }
}