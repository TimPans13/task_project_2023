using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using System;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public interface IProviderFillwordLevel
    {
        GridFillWords LoadModel(int index);
        event Action<int> OnLoadModelRequested;
    }
}