using System;
using UnityEngine;

public class DefenderToolbar : MonoBehaviour {

    [SerializeField] private GameObject trophyButton;
    [SerializeField] private GameObject doubleTrophyButton;
    [SerializeField] private GameObject cactusButton;
    [SerializeField] private GameObject scarecrowButton;
    [SerializeField] private GameObject statueButton;
    [SerializeField] private GameObject gravestoneButton;
    [SerializeField] private GameObject vendorButton;
    [SerializeField] private GameObject gnomeButton;
    
    private DefenderType[] _defenders;
    
    private int _order = 2;

    private LevelController _levelController;
    
    private void Start() {
        _levelController = FindObjectOfType<LevelController>();
        _levelController.OnLevelStart += StartGame;
    }

    private void StartGame(object sender, EventArgs e) {
        foreach (var defenderButton in _defenders) {
            switch (defenderButton) {
                case DefenderType.Trophy:
                    trophyButton.SetActive(true);
                    trophyButton.transform.localPosition = new Vector2(_order, trophyButton.transform.localPosition.y);
                    break;
                case DefenderType.DoubleTrophy:
                    doubleTrophyButton.SetActive(true);
                    doubleTrophyButton.transform.localPosition = new Vector2(_order, doubleTrophyButton.transform.localPosition.y);
                    break;
                case DefenderType.Cactus:
                    cactusButton.SetActive(true);
                    cactusButton.transform.localPosition = new Vector2(_order, cactusButton.transform.localPosition.y);
                    break;
                case DefenderType.Scarecrow:
                    scarecrowButton.SetActive(true);
                    scarecrowButton.transform.localPosition = new Vector2(_order, scarecrowButton.transform.localPosition.y);
                    break;
                case DefenderType.Statue:
                    statueButton.SetActive(true);
                    statueButton.transform.localPosition = new Vector2(_order, statueButton.transform.localPosition.y);
                    break;
                case DefenderType.Gravestone:
                    gravestoneButton.SetActive(true);
                    gravestoneButton.transform.localPosition = new Vector2(_order, gravestoneButton.transform.localPosition.y);
                    break;
                case DefenderType.Vendor:
                    vendorButton.SetActive(true);
                    vendorButton.transform.localPosition = new Vector2(_order, vendorButton.transform.localPosition.y);
                    break;
                case DefenderType.Gnome:
                    gnomeButton.SetActive(true);
                    gnomeButton.transform.localPosition = new Vector2(_order, gnomeButton.transform.localPosition.y);
                    break;
                case DefenderType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _order++;
        }
    }

    public void SetDefenders(DefenderType[] defenders) {
        _defenders = defenders;
    }
    
    
}
