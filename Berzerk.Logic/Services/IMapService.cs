using System.Windows.Forms;

namespace Berzerk.Logic.Services
{
    public interface IMapService
    {
        void LoadMap(Form form, int level);
        bool IsLastMap(int level);
    }
}