using UnityEditor;
using UnityEngine;
using naxokit.Helpers.Configs;

namespace naxokit {

    [InitializeOnLoad]
    class Naxoinit {
        static Naxoinit() {
            Config.InitializeCofig();
        }
    }
}