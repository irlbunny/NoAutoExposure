using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NoAutoExposure
{
    internal class Config
    {
        public virtual bool DisableToneMapping { get; set; }
    }
}
