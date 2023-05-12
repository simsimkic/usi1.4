using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using System.Text;
namespace ZdravoCorp.Serialization

{
    public interface Serializable
    {

        string[] ToCSV();

        void FromCSV(string[] values);

    }
}