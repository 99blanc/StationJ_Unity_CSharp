using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ViewNavPopup : UIPopup
{
    private enum RawImages
    {
        XRImage,
        RawImage
    }

    private enum Images
    {
        Mask,

        Button_01,
        Button_02,
        Button_03,
        Button_04
    }

    private enum Texts
    {
        StartText,
        EndText,
    }

    private enum Buttons
    {
        CurrentPosIcon,
        NavModeIcon,

        CancelButton
    }

    public override void Init()
    {
        base.Init();

        BindRawImage(typeof(RawImages));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        StartCoroutine(Managers.App.startLatLon.GetAddress(response =>
        {
            LocationDetails json = JsonUtilities.JsonToObject<LocationDetails>(response);

            foreach (ResourceLocationSet resourceSet in json.resourceSets)
            {
                foreach (ResourceLocation resource in resourceSet.resources)
                {
                    if (false == string.IsNullOrEmpty(resource.address.formattedAddress))
                    {
                        GetText((int)Texts.StartText).text = resource.address.formattedAddress;
                    }
                }
            }
        }));

        StartCoroutine(Managers.App.endLatLon.GetAddress(response =>
        {
            LocationDetails json = JsonUtilities.JsonToObject<LocationDetails>(response);

            foreach (ResourceLocationSet resourceSet in json.resourceSets)
            {
                foreach (ResourceLocation resource in resourceSet.resources)
                {
                    if (false == string.IsNullOrEmpty(resource.address.formattedAddress))
                    {
                        GetText((int)Texts.EndText).text = resource.address.formattedAddress;
                    }
                }
            }
        }));

        GetRawImage((int)RawImages.RawImage).BindViewEvent(OnDragRawImage, ViewEvent.Drag, this);

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        SetRouteMode();
        UpdateLatLon();

        LatLon latLon = Managers.App.MapLocationService.GetLatLon();
        if (latLon != default)
        {
            GetRawImage((int)RawImages.XRImage).texture = Managers.Resource.LoadRenderTexture("XRCamera");
        }
        else
        {
            GetRawImage((int)RawImages.XRImage).texture = Managers.Resource.LoadSprite("spr_DownloadBackground").texture;
        }

        Managers.Sound.Stop(SoundType.BGM);
        Managers.Sound.Play(SoundID.RoadBGM);
        Managers.UI.OpenPopup<FindPathPopup>();
    }

    private void OnClickButton(PointerEventData eventData)
    {
        Buttons button = Enum.Parse<Buttons>(eventData.pointerEnter.name);

        ProcessButton(button);
    }

    private void ProcessButton(Buttons button)
    {
        switch (button)
        {
            case Buttons.CurrentPosIcon:
                LatLon latLon = Managers.App.MapLocationService.GetLatLon();
                if (latLon != default)
                {
                    Managers.App.MapRenderer.Center = latLon;
                }
                else
                {
                    Managers.UI.OpenPopup<ConsentPopup>();
                }
                break;
            case Buttons.NavModeIcon:
                Managers.App.SetNavMode();
                break;
            case Buttons.CancelButton:
                Managers.App.MapPinLayer.MapPins.Clear();
                Managers.App.MapPinSubLayer.MapPins.Clear();
                Managers.App.MapLineRenderer.gameObject.SetActive(false);
                Managers.App.updateLatLon = false;
                Managers.App.PopupPinLayer.MapPins.Remove(Managers.App.MapPin);
                Managers.App.BingRouteMode = BingRouteMode.None;
                Managers.UI.ClosePopupUI();
                Managers.Sound.Stop(SoundType.BGM);
                Managers.Sound.Play(SoundID.MainBGM);
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnDragRawImage(PointerEventData eventData)
    {
        Vector2 dragDelta = -eventData.delta * Managers.App.polatedValue;

        Managers.App.MapController.Pan(dragDelta, false);
    }

    private void SetRouteMode()
    {
        switch (Managers.App.BingRouteMode)
        {
            case BingRouteMode.Driving:
                GetImage((int)Images.Button_01).sprite = Managers.Resource.LoadSprite("spr_CasrIconHover");
                break;
            case BingRouteMode.Walking:
                GetImage((int)Images.Button_02).sprite = Managers.Resource.LoadSprite("spr_HumanIconHover");
                break;
            case BingRouteMode.Transit:
                GetImage((int)Images.Button_03).sprite = Managers.Resource.LoadSprite("spr_BusIconHover");
                break;
            case BingRouteMode.Bicycling:
                GetImage((int)Images.Button_04).sprite = Managers.Resource.LoadSprite("spr_BicycleIconHover");
                break;
        }
    }

    private void UpdateLatLon()
    {
        Managers.App.updateLatLon = true;
        MapPin mapPin = Managers.Resource.Instantiate("CurrentPin").GetComponent<MapPin>();
        Managers.App.MapPin = mapPin;
        mapPin.Location = Managers.App.MapLocationService.GetLatLon();
        mapPin.IsLayerSynchronized = false;
        Managers.App.PopupPinLayer.MapPins.Add(mapPin);
    }
}
