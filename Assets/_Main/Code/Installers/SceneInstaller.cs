using OKRT.TechnokomTestTask.Main.Code.Data;
using OKRT.TechnokomTestTask.Main.Code.Models;
using UnityEngine;
using Zenject;

namespace OKRT.TechnokomTestTask.Main.Code.Installers
{
    /// <summary>
    /// The <see cref="SceneInstaller"/> class.
    /// This is Zenject's scene installer.
    /// </summary>
    internal sealed class SceneInstaller : MonoInstaller
    {
        #region Private

        [SerializeField] private SceneData _sceneData;

        #endregion

        #region Methods

        public override void InstallBindings()
        {
            Container.BindInstance(_sceneData).AsSingle();
            Container.BindInstance(_sceneData.PlayerData).AsSingle();
            Container.Bind<ShopPurchaseModel>().AsSingle().NonLazy();
        }

        #endregion
    }
}
