using PH_Swag.Models;

namespace PH_Swag.Repository
{
    public interface IMerchandiseRepository
    {
        public Product GetUser(int id);

        public Image GetImages(int id);
    }
}
