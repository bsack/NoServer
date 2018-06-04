using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
//using System.Net;
//using System.IO;

[Serializable] //makes it possible to go from JSON to object, variables below must match what is in JSON data

public class TelemetryData
{
    //need to add p_int
    public float p_sub;       // SUB PRESSURE [psia] - External Environment pressure - Expected range is from 2 to 4 psia
    //need to add battery life, oxygen life, water life
    public float t_sub;       // SUB TEMPERATURE [degrees Fahrenheit] - External Environmental temperature measured in degrees Fahrenheit
    public int v_fan;         // FAN TACHOMETER [RPM] - Speed of the cooling fan - Expected range is from 10000 to 40000 RPM
    public string t_eva;      // EXTRAVEHICULAR ACTIVITY TIME [time value] - Stopwatch for the current EVA. Important indicator for astronauts to monitor current process of the task on-hand.
    public int p_o2;          // OXYGEN PRESSURE [psia] - Pressure inside the Primary Oxygen Pack - Expected range is from 750 to 950 psia.
    public double rate_o2;     // OXYGEN RATE [psi/min] - Flowrate of the Primary Oxygen Pack - Expected range is from 0.5 to 1 psi/min
    public float cap_battery; // BATTERY CAPACITY [amp-hr] - Total capacity of the spacesuit’s battery - Expected range is from 0 to 30 amp-hr
    public float p_h2o_g;     // H2O GAS PRESSURE [psia] - Gas pressure from H2O system - Expected range is from 14 to 16 psia
    public float p_h2o_l;     // H2O LIQUID PRESSURE [psia] - Liquid pressure from H2O system - Expected range is from 14 to 16 psia
    public float p_sop;       // SOP PRESSURE [psia] - Pressure inside the Secondary Oxygen Pack - Expected range is from 750 to 950 psia.
    public float rate_sop;    // SOP RATE [psi/min] - Flowrate of the Secondary Oxygen Pack - Expected range is from 0.5 to 1 psi/min.
}

public class DisplayedTelemetryFields
{
    //need to add p_int
    public Boolean p_sub;
    //need to add battery life, oxygen life, water life
    public Boolean t_sub;
    public Boolean v_fan;
    public Boolean t_eva;
    public Boolean p_o2;
    public Boolean rate_o2;
    public Boolean cap_battery;
    public Boolean p_h2o_g;
    public Boolean p_h2o_l;
    public Boolean p_sop;
    public Boolean rate_sop;
}

[Serializable] //same thing as before when you get data, serialize into this class (as if its correctly doing T/F) ---fix to be correct in app!!!! (remove quotes)
public class SwitchData
{
    public Boolean sop_on;
    public Boolean sspe;
    public Boolean fan_error;
    public Boolean vent_error;
    public Boolean vehicle_power;
    public Boolean h2o_off;
    public Boolean o2_off;
    //must add alarm variables
}

public class NewNewScript : MonoBehaviour
{

    //Text telemetryBox; //declaring text object to be used later to set equal to the "Telemetry" object we find
    Text p_sub, t_sub, v_fan, t_eva, p_o2, rate_o2, cap_battery, p_h2o_g, p_h2o_l, p_sop, rate_sop;
    Text alertsBox;

    public TelemetryData telemetrydata = new TelemetryData(); //telemtrydata is a new instance of the TelemetryData Class above (whats gonna hold values from stream)
    SwitchData switchdata = new SwitchData();
    DisplayedTelemetryFields displayedtelemetryfields = new DisplayedTelemetryFields();

    //static string telemetryServer = "http://127.0.0.1"; //first part of server

    //string telemetryURL = telemetryServer + "/api/telemetry/recent"; //second part
    //string switchURL = telemetryServer + "/api/switch/recent";

    //float elapsedSecs = 0.0f;           // Overall counter (currently not used)
    static float refreshSecs = 10.0f;    // How often to refresh (GET) the data from the telemetry server (seconds) - used later to do a refresh in the beginning, must be static cuz its initial
    float refreshTimer = refreshSecs;   // Timer used to trigger a data refresh (initialized to match so we immediately refresh) - used for refresh after

    // Use this for initialization
    void Start()
    {
        Debug.Log("Initializing ..."); //message to console to tell its running

        // Find the "Telemetry" Text canvas object and assign it to telemetryBox
        // The position, initial message, positioning, font, etc are all defined in the Unity Inspector
        if ((p_sub == null) && (transform.Find("Alerts") != null)) //if we havent assigned anything to telemetryBox and havent found Telemetry then... 
        {
            //Transform child = transform.Find("Telemetry"); //define a Transform object named "child" set it equal to the UI text element called Telemetry 
            //telemetryBox = child.GetComponent<Text>(); //Get the text component of the child thing above and set equal to the text class object "telemetryBox" - this is what we change each time to change text
            //Debug.Log(telemetryBox.text); 

            Transform child = transform.Find("P_sub");
            p_sub = child.GetComponent<Text>();

            child = transform.Find("T_sub");
            t_sub = child.GetComponent<Text>();

            child = transform.Find("V_fan");
            v_fan = child.GetComponent<Text>();

            child = transform.Find("EVA Time Elapsed");
            t_eva = child.GetComponent<Text>();

            child = transform.Find("Oxygen Pressure");
            p_o2 = child.GetComponent<Text>();

            child = transform.Find("Oxygen Rate");
            rate_o2 = child.GetComponent<Text>();

            child = transform.Find("Battery Capacity");
            cap_battery = child.GetComponent<Text>();

            child = transform.Find("H2O Gas Pressure");
            p_h2o_g = child.GetComponent<Text>();

            child = transform.Find("H2O Liquid Pressure");
            p_h2o_l = child.GetComponent<Text>();

            child = transform.Find("SOP Pressure");
            p_sop = child.GetComponent<Text>();

            child = transform.Find("SOP Rate");
            rate_sop = child.GetComponent<Text>();

        }

        // Find the "Alerts" Text canvas object and assign it to alertsBox
        if ((alertsBox == null) && (transform.Find("Alerts") != null)) //same as above but switches
        {
            Transform child = transform.Find("Alerts");
            alertsBox = child.GetComponent<Text>();
            Debug.Log(alertsBox.text);
        }

        //Setting all of the boolean metric variables to be true to display everything
        //need to add p_int
        displayedtelemetryfields.p_sub = true;
        //need to add battery life, oxygen life, water life
        displayedtelemetryfields.t_sub = true;
        displayedtelemetryfields.v_fan = true;
        displayedtelemetryfields.t_eva = true;
        displayedtelemetryfields.p_o2 = true;
        displayedtelemetryfields.rate_o2 = true;
        displayedtelemetryfields.cap_battery = true;
        displayedtelemetryfields.p_h2o_g = true;
        displayedtelemetryfields.p_h2o_l = true;
        displayedtelemetryfields.p_sop = true;
        displayedtelemetryfields.rate_sop = true;

    }

    // Update is called once per frame
    void Update()
    {

        if ((p_sub == null) || (alertsBox == null)) { return; }  // If telemetryBox or alertsBox has not been initialized above in start then just return...(too soon, not initialized yet)

        // Track overall elapsed time
        //elapsedSecs += Time.deltaTime;

        // Increment timer with time elapsed since the last update
        refreshTimer += Time.deltaTime; //time delta is basically framerate (how often update happens)

        if (refreshTimer > refreshSecs)
        {                    //refreshTimer is now how long its been and refreshSecs is still 10 so if timer > 10 do the following
            // Time to refresh the data, reset the timer and get new suit data
            refreshTimer = 0; //reset the timer
            telemetrydata = RefreshData(); //refresh the data
            //telemetryBox.text = "Sub Pressure: " + telemetrydata.p_sub.ToString() + "Sub Temperature: " + telemetrydata.t_sub.ToString();
            p_sub.text = "Sub Pressure: " + telemetrydata.p_sub.ToString();
            t_sub.text = "Sub Temperature: " + telemetrydata.t_sub.ToString();
            v_fan.text = "Fan Speed: " + telemetrydata.v_fan.ToString();
            //t_eva.text = "Time Elapsed: " + telemetrydata.t_eva.ToString();
            p_o2.text = "Oxygen Pressure" + telemetrydata.p_o2.ToString();
            rate_o2.text = "Oxygen Flow Rate: " + telemetrydata.rate_o2.ToString();
            cap_battery.text = "Battery Capacity: " + telemetrydata.cap_battery.ToString();
            p_h2o_g.text = "H2O Gas Pressure: " + telemetrydata.p_h2o_g.ToString();
            p_h2o_l.text = "H2O Liquid Pressure: " + telemetrydata.p_h2o_l.ToString();
            p_sop.text = "Secondary Oxygen Pressure: " + telemetrydata.p_sop.ToString();
            rate_sop.text = "Secondary Oxygen Flow Rate: " + telemetrydata.rate_sop.ToString();



         
       

            //----add if statements to evaluate if metrics are in alarm state ---


            // Update displayed telemetry data
            List<string> metrics = new List<string>(); //defining a new class type list and calling it metrics, it will be a list of strings

            //below was me hard coding adding telemetry metrics to a list to be displayed
            //metrics.Add("Sub Pressure: " + telemetrydata.p_sub.ToString()); //add to metrics this string plus its value converted to a string
            //metrics.Add("Env Temp: " + telemetrydata.t_sub.ToString());

            //-----here is where we will set certan metrics to true if we get a voice command------

            //if the boolean metrics are set to true then show them
            //need to add p_int


            /*if (displayedtelemetryfields.p_sub) { metrics.Add("Sub Pressure: " + telemetrydata.p_sub.ToString()); }
            //need to add battery life, water life, oxygen life
            if (displayedtelemetryfields.t_sub) { metrics.Add("Sub Temperature: " + telemetrydata.t_sub.ToString()); }
            if (displayedtelemetryfields.v_fan) { metrics.Add("Fan Velocity: " + telemetrydata.v_fan.ToString()); }
            if (displayedtelemetryfields.t_eva) { metrics.Add("EVA Time: " + telemetrydata.t_eva); }
            if (displayedtelemetryfields.p_o2) { metrics.Add("Oxygen Pressure: " + telemetrydata.p_o2.ToString()); }
            if (displayedtelemetryfields.rate_o2) { metrics.Add("Oxygen Rate: " + telemetrydata.rate_o2.ToString()); }
            if (displayedtelemetryfields.cap_battery) { metrics.Add("Battery Capacity: " + telemetrydata.cap_battery.ToString()); }
            if (displayedtelemetryfields.p_h2o_g) { metrics.Add("Water Vapor Pressure: " + telemetrydata.p_h2o_g.ToString()); }
            if (displayedtelemetryfields.p_h2o_l) { metrics.Add("Water Liquid Pressure: " + telemetrydata.p_h2o_l.ToString()); }
            if (displayedtelemetryfields.p_sop) { metrics.Add("SOP Pressure: " + telemetrydata.p_sop.ToString()); }
            if (displayedtelemetryfields.rate_sop) { metrics.Add("SOP Rate: " + telemetrydata.rate_sop.ToString()); }

            telemetryBox.text = String.Join("\n", metrics.ToArray()); //take metrics list, convert it to an array. Then take that array and join it together with new lines. Store it as the text component of telemetryBox

            // Check for and display any alerts
            List<string> alerts = new List<string>(); // same as before now a list of strings for the alerts
            if (switchdata.sop_on) { alerts.Add("Secondary Oxygen ACTIVE"); } //if the switches are true then add the alert to the list
            if (switchdata.sspe) { alerts.Add("Spacesuit Pressure EMERGENCY"); }
            if (switchdata.fan_error) { alerts.Add("Fan FAILURE"); }
            if (switchdata.vent_error) { alerts.Add("Vent ERROR"); }
            if (switchdata.vehicle_power) { alerts.Add("Vehicle Power PRESENT"); }
            if (switchdata.h2o_off) { alerts.Add("H2O System OFFLINE"); }
            if (switchdata.o2_off) { alerts.Add("O2 System OFFLINE"); }
            alertsBox.text = String.Join("\n", alerts.ToArray()); //rewrite the entire alertbox text component to be this string with new lines in between the elements of the alerts list which have been converted to an array (like before)
            */
        }

    }

    public TelemetryData RefreshData()
    {
        TelemetryData up = new TelemetryData();
        System.Random r = new System.Random();

        up.p_sub = r.Next(2, 4);
        up.t_sub = r.Next(-250, 250);
        up.v_fan = r.Next(10000, 40000);
        up.p_o2 = r.Next(750, 950);
        up.rate_o2 = r.Next(5, 10) / 10;
        up.cap_battery = r.Next(0, 30);
        up.p_h2o_g = r.Next(14, 16);
        up.p_h2o_l = r.Next(14, 16);
        up.p_sop = r.Next(750, 950);
        up.rate_sop = r.Next(5, 10) / 10;


        return up;
    }
}

  


