using System;
using System.Threading.Tasks;
using Data;
using UnityEngine;

public class DataForChampDisplay : MonoBehaviour
{
    private IDataProvider dataProvider;
    private Awaitable dataAwaitable;
    private StandingsProcessor standingsProcessor;

    [SerializeField] private string[] classes;
    [SerializeField] private ChampDisplay display;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        dataProvider = new GoogleSheetsDataProvider("1sm6MHigYRdGtapSMlMq5aCvB0oyxnlG3gUPn_F1dRkY", classes);
        standingsProcessor = new StandingsProcessor();
        dataProvider.RefreshedDataReady += DataProviderOnRefreshedDataReady;
    }

    private void DataProviderOnRefreshedDataReady(object sender, EventArgs e)
    {
	    var data = dataProvider.TryGetAllDriverData();
	    if (data == null)
	    {
		    Debug.LogError("Failed to get Data!");
	    }
	    
	    standingsProcessor.UpdateData(data);

	    foreach (string carClass in classes)
	    {
		    display.CreateClass(carClass, standingsProcessor.GetSortedResultsForClass(carClass));
	    }
    }

    //called when the overlay gets enabled
    private async void OnEnable()
    {
       await dataProvider.UpdateData();
    }
}
