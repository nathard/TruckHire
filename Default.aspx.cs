using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Net;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.XPath;
using System.Threading;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using TopicPublisher;
using TopicSubscriber;

public partial class _Default : System.Web.UI.Page
{

    public static bool MemberExists;
    public static bool OrderExists;
    public static double price;
    // SQL globals
    public static SqlConnection conn = new SqlConnection(
                @"Data Source=.\SQLEXPRESS; AttachDbFilename='|DataDirectory|\employees.mdf';
                Integrated Security=True; User Instance=True");
    public static SqlCommand cmd = conn.CreateCommand();
    public static SqlTransaction transaction;
    // message queue globals
    private IConnection connection;
    private IConnectionFactory connectionFactory;
    private Subscriber subscriber;
    private ISession session;
    const string TOPIC_NAME = "testing";
    private const string BROKER = "tcp://localhost:61616";
    const string CLIENT_ID = "test.clientId";
    const string CONSUMER_ID = "test.subscriber";

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    // Check Public Holiday status
    public static bool IsPublicHoliday(DateTime _phDate, string _state)
    {

    string phDate = _phDate.ToShortDateString();
	switch (_state)
            {
                case "ACT":
					string[] _ACT = { "1/01/2013", "26/01/2013", "11/03/2013", "29/03/2013", "30/03/2013", "31/03/2013", "1/04/2013", "25/04/2013", "18/04/2014", "20/04/2014", "21/04/2014", "10/06/2013", "7/10/2013", "25/12/2013", "26/12/2013" };
                    if (_ACT.Contains(phDate))
                    {
                        return true;
					} else goto default;
                    
                case "NSW":
                    string[] _NSW = { "1/01/2013", "26/01/2013", "29/03/2013", "30/03/2013", "31/03/2013", "1/04/2013", "18/04/2014", "20/04/2014", "21/04/2014", "25/04/2013", "10/06/2013", "7/10/2013", "25/12/2013", "26/12/2013" };
                    if (_NSW.Contains(phDate))
                    {
                        return true;
					} else goto default;
                
                case "VIC":
                    string[] _VIC = { "1/01/2013", "26/01/2013", "11/03/2013", "29/03/2013", "30/03/2013", "31/03/2013", "1/04/2013", "18/04/2014", "20/04/2014", "21/04/2014", "25/04/2013", "10/06/2013", "5/11/2013", "25/12/2013", "26/12/2013" };
                    if (_VIC.Contains(phDate)) 
                    {                       
					    return true;
					} else goto default;
                case "NT":
                    string[] _NT = { "1/01/2013", "26/01/2013", "29/03/2013", "30/03/2013", "31/03/2013", "1/04/2013", "18/04/2014", "20/04/2014", "21/04/2014", "25/04/2013", "6/05/2013", "10-06-2013", "5/08/2013", "7/10/2013", "25/12/2013", "26/12/2013" };
                    if (_NT.Contains(phDate))
                    {
                        return true;
                    } else goto default;
                case "QLD":
                    string[] _QLD = { "1/01/2013", "6/01/2013", "29/03/2013", "30/03/2013", "31/03/2013", "1/04/2013", "18/04/2014", "20/04/2014", "21/04/2014", "25/04/2013", "10-06-2013", "7/10/2013", "25/12/2013", "26/12/2013" };
                    if (_QLD.Contains(phDate))
                    {
                        return true;
                    } else goto default;
                case "SA":
                    string[] _SA = { "1/01/2013", "26/01/2013", "29/03/2013", "11/03/2013", "30/03/2013", "31/03/2013", "1/04/2013", "18/04/2014", "20/04/2014", "21/04/2014", "25/04/2013", "10/06/2013", "7/10/2013", "25/12/2013", "26/12/2013", "31/12/2013" };
                    if (_SA.Contains(phDate))
                    {
                        return true;
                    } else goto default;
                case "TAS":
                    string[] _TAS = { "1/01/2013", "26/01/2013", "11/02/2013", "11/03/2013", "29/03/2013", "30/03/2013", "31/03/2013", "1/04/2013", "18/04/2014", "20/04/2014", "21/04/2014", "2/04/2013", "25/04/2013", "10/06/2013", "4/11/2013", "25/12/2013", "26/12/2013" };
                    if (_TAS.Contains(phDate))
                    {
                        return true;
                    } else goto default;

                case "WA":
                    string[] _WA = { "1/01/2013", "26/01/2013", "11/03/2013", "29/03/2013", "30/03/2013", "31/03/2013", "1/04/2013", "18/04/2014", "20/04/2014", "21/04/2014", "25/04/2013", "3/06/2013", "5/11/2013", "25/12/2013", "26/12/2013" };
                    if (_WA.Contains(phDate))
                    {
                        return true;
                    } else goto default;
                default:
                    return false;
                    
            }
    }
    // Check if date/time falls between time spans
    public static bool IsTimeOfDayBetween(DateTime _time, TimeSpan startTime, TimeSpan endTime)
    {
        if (endTime == startTime)
        {
            return true;
        }
        else if (endTime < startTime)
        {
            return _time.TimeOfDay <= endTime ||
                _time.TimeOfDay >= startTime;
        }
        else
        {
            return _time.TimeOfDay >= startTime &&
                _time.TimeOfDay <= endTime;
        }

    }
    // Check if dates fall between days of the week
    public static bool IsDayOfWeekBetween(DateTime _date, DayOfWeek start, DayOfWeek end)
    {
        DayOfWeek curDay = _date.DayOfWeek;

        if (start <= end)
        {
            // Test one range: start to end
            return (start <= curDay && curDay <= end);
        }
        else
        {
            // Test two ranges: start to 6, 0 to end
            return (start <= curDay || curDay <= end);
        }
    }

    protected void buttonValidate(Object Sender, EventArgs E)
    {
        Page.Validate();
        if (Page.IsValid == true) {
            // 
            // Member creation
            //
            #region Member
            if (!MemberExists)
            {

                conn.Open();

                transaction = conn.BeginTransaction("MemberTransaction");

                cmd.Connection = conn;
                cmd.Transaction = transaction;
                // Insertion Placeholders
                string[] FullName = txtFullName3.Text.Split(' ');

                cmd.Parameters.AddWithValue("@FirstName", FullName[0]);
                cmd.Parameters.AddWithValue("@LastName", FullName[1]);
                cmd.Parameters.AddWithValue("@MemClass", "Regular");
                cmd.Parameters.AddWithValue("@MemId", txtMemberId.Text);
                cmd.Parameters.AddWithValue("@BirthDate", DateTime.Parse("01-01-1950"));
                cmd.Parameters.AddWithValue("@Salary", 65000);

                // Verfication Code
                string VerCode = 65000 + "," + DateTime.Parse("01-01-1950");
                cmd.Parameters.AddWithValue("@VerCode", VerCode);

                try
                {
                    cmd.CommandText =
                        "INSERT INTO MEMBERSHIP (MembershipId,FirstName,LastName,MembershipClass,VerificationCode,birth_date,salary) VALUES (@MemId,@FirstName,@LastName,@MemClass,@VerCode,@BirthDate,@Salary)";
                    cmd.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();
                    lblMember.Text = "User added successfully to database.";
                    MemberExists = true;
                }
                catch (Exception ex)
                {
                    lblMember.Text = ex.Message;

                    // Attempt to roll back the transaction. 
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        lblMember.Text = ex2.Message;
                    }
                }
                
                conn.Close();
            }

            #endregion
            //
            // Distance calculation logic
            //
            #region Distance
            
            DateTime date = Convert.ToDateTime(txtDate.Text + " " + txtTime.Text);
            string oState = txtState1.Text;
            string dState = txtState2.Text;
            /* Origin: */
            #region origin
            // set capital city, rates and peak times for origin
            string oCapital = "";
            TimeSpan offpeak = new TimeSpan();
            TimeSpan onpeak = new TimeSpan();
            double[] rates = new double[6];

            switch (oState)
            {
                case "ACT":
                    oCapital = "Canberra 2600";
                    rates[0] = 200; rates[1] = 250; rates[2] = 325; rates[3] = 360; rates[4] = 400; rates[5] = 480;
                    onpeak = new TimeSpan(5, 0, 0); offpeak = new TimeSpan(17, 00, 0);
                    break;
                case "NSW":
                    oCapital = "Sydney 2000";
                    rates[0] = 195; rates[1] = 245; rates[2] = 315; rates[3] = 350; rates[4] = 390; rates[5] = 465;
                    onpeak = new TimeSpan(5, 0, 0); offpeak = new TimeSpan(17, 00, 0);
                    break;
                case "NT":
                    oCapital = "Darwin 0800";
                    rates[0] = 150; rates[1] = 180; rates[2] = 225; rates[3] = 260; rates[4] = 235; rates[5] = 270;
                    onpeak = new TimeSpan(7, 0, 0); offpeak = new TimeSpan(19, 00, 0);
                    break;
                case "QLD":
                    oCapital = "Brisbane 4000";
                    rates[0] = 175; rates[1] = 225; rates[2] = 195; rates[3] = 280; rates[4] = 385; rates[5] = 400;
                    onpeak = new TimeSpan(6, 0, 0); offpeak = new TimeSpan(18, 00, 0);
                    break;
                case "SA":
                    oCapital = "Adelaide 5000";
                    rates[0] = 195; rates[1] = 270; rates[2] = 390; rates[3] = 410; rates[4] = 420; rates[5] = 500;
                    onpeak = new TimeSpan(6, 0, 0); offpeak = new TimeSpan(18, 00, 0);
                    break;
                case "TAS":
                    oCapital = "Hobart 7000";
                    rates[0] = 178; rates[1] = 214; rates[2] = 225; rates[3] = 269; rates[4] = 225; rates[5] = 269;
                    onpeak = new TimeSpan(7, 0, 0); offpeak = new TimeSpan(19, 00, 0);
                    break;
                case "VIC":
                    oCapital = "Melbourne 3000";
                    rates[0] = 194; rates[1] = 244; rates[2] = 330; rates[3] = 348; rates[4] = 410; rates[5] = 410;
                    onpeak = new TimeSpan(6, 0, 0); offpeak = new TimeSpan(18, 00, 0); 
                    break;
                case "WA":
                    oCapital = "Perth 6000";
                    rates[0] = 175; rates[1] = 225; rates[2] = 195; rates[3] = 280; rates[4] = 385; rates[5] = 405;
                    onpeak = new TimeSpan(7, 0, 0); offpeak = new TimeSpan(19, 00, 0);
                    break;
                default:

                    break;
            }
            // calculate origin distance from capital using google api
            
            string xmlResult = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + txtCity1.Text + txtState1.Text + txtPostCode1.Text + "&destinations=" + oCapital + "&mode=Car&language=us-en&sensor=false");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader resStream = new StreamReader(response.GetResponseStream());
            XmlDocument doc = new XmlDocument();
            xmlResult = resStream.ReadToEnd();
            doc.LoadXml(xmlResult);

            try
            {
                if (doc.DocumentElement.SelectSingleNode("/DistanceMatrixResponse/row/element/status").InnerText.ToString().ToUpper() != "OK")
                {
                    lblResult.Text = "Invalid Origin Address please try again";
                    return;
                }
                
                
            }
            catch (Exception ex)
            {
                lblResult.Text = "Error during processing";
                return;
            }

            // Convert Origin distance into number
            string _originDistance = doc.DocumentElement.SelectSingleNode("/DistanceMatrixResponse/row/element/distance/value").InnerText;
            double originDistance = Convert.ToDouble(_originDistance);
            originDistance = originDistance * 0.001;    // Convert to Kilometers

            // Calculate weightings for travel duration
            // 100km's = 1 hour or rounded closest to, anything less than 100km's is fixed to 1 hour
            double originTime = 0;

            if (originDistance <= 100)
            {
                originTime = 1;
            }
            else
                originTime = Math.Round(originDistance * 0.01);
            
            #endregion
            /* Destination: */
            #region destination
            // Set capital city for destination
            string dCapital = "";
            switch (dState)
            {
                case "ACT":
                    dCapital = "Canberra 2600";
                    rates[0] = 200; rates[1] = 250; rates[2] = 325; rates[3] = 360; rates[4] = 400; rates[5] = 480;
                    onpeak = new TimeSpan(5, 0, 0); offpeak = new TimeSpan(17, 00, 0);
                    break;
                case "NSW":
                    dCapital = "Sydney 2000";
                    rates[0] = 195; rates[1] = 245; rates[2] = 315; rates[3] = 350; rates[4] = 390; rates[5] = 465;
                    onpeak = new TimeSpan(5, 0, 0); offpeak = new TimeSpan(17, 00, 0);
                    break;
                case "NT":
                    dCapital = "Darwin 0800";
                    rates[0] = 150; rates[1] = 180; rates[2] = 225; rates[3] = 260; rates[4] = 235; rates[5] = 270;
                    onpeak = new TimeSpan(7, 0, 0); offpeak = new TimeSpan(19, 00, 0);
                    break;
                case "QLD":
                    dCapital = "Brisbane 4000";
                    rates[0] = 175; rates[1] = 225; rates[2] = 195; rates[3] = 280; rates[4] = 385; rates[5] = 400;
                    onpeak = new TimeSpan(6, 0, 0); offpeak = new TimeSpan(18, 00, 0);
                    break;
                case "SA":
                    dCapital = "Adelaide 5000";
                    rates[0] = 195; rates[1] = 270; rates[2] = 390; rates[3] = 410; rates[4] = 420; rates[5] = 500;
                    onpeak = new TimeSpan(6, 0, 0); offpeak = new TimeSpan(18, 00, 0);
                    break;
                case "TAS":
                    dCapital = "Hobart 7000";
                    rates[0] = 178; rates[1] = 214; rates[2] = 225; rates[3] = 269; rates[4] = 225; rates[5] = 269;
                    onpeak = new TimeSpan(7, 0, 0); offpeak = new TimeSpan(19, 00, 0);
                    break;
                case "VIC":
                    dCapital = "Melbourne 3000";
                    rates[0] = 194; rates[1] = 244; rates[2] = 330; rates[3] = 348; rates[4] = 410; rates[5] = 410;
                    onpeak = new TimeSpan(6, 0, 0); offpeak = new TimeSpan(18, 00, 0);
                    break;
                case "WA":
                    dCapital = "Perth 6000";
                    rates[0] = 175; rates[1] = 225; rates[2] = 195; rates[3] = 280; rates[4] = 385; rates[5] = 405;
                    onpeak = new TimeSpan(7, 0, 0); offpeak = new TimeSpan(19, 00, 0);
                    break;
                default:
                    break;
            }

            // calculate desitination distance
            
            xmlResult = null;
            request = (HttpWebRequest)WebRequest.Create("http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + txtCity2.Text + txtState2.Text + txtPostCode2.Text + "&destinations=" + dCapital + "&mode=Car&language=us-en&sensor=false");
            response = (HttpWebResponse)request.GetResponse();
            resStream = new StreamReader(response.GetResponseStream());
            doc = new XmlDocument();
            xmlResult = resStream.ReadToEnd();
            doc.LoadXml(xmlResult);

            try
            {
                if (doc.DocumentElement.SelectSingleNode("/DistanceMatrixResponse/row/element/status").InnerText.ToString().ToUpper() != "OK")
                {
                    lblResult.Text = "Invalid Destination Address please try again";
                    return;
                }


            }
            catch (Exception ex)
            {
                lblResult.Text = "Error during processing";
                return;
            }

            // Convert distance string number
            string _destDistance = doc.DocumentElement.SelectSingleNode("/DistanceMatrixResponse/row/element/distance/value").InnerText;
            double destDistance = Convert.ToDouble(_destDistance);
            destDistance = destDistance * 0.001; // Convert to kilometers
            
            // Calculate weightings for travel duration
            // 100km's = 1 hour 
            double destTime = 0;

            if (destDistance <= 100)
            {
                destTime = 1;
            }
            else
                destTime = Math.Round(destDistance * 0.01);

            #endregion
            /* Interstate: */
            #region interstate
            // Fixed route distances used for interstate travel
            // set to approximate duration (hours)

            const int FR1_MelbSyd = 9;
            const int FR2_MelbHob = 22;
            const int FR3_MelbAdel = 8;
            const int FR4_SydCan = 3;
            const int FR5_SydBris = 10;
            const int FR6_BrisDar = 40;
            const int FR7_DarPerth = 43;
            const int FR8_AdelDar = 35;
            const int FR9_PerthAdel = 28;
            
            // Timezone Scenarios
            double AWST = 1.5;
            double ACST = 0.5;
            double AEST = ACST + AWST;

            // calulate intrastate travel
            double totalDuration = 0;
            DateTime arrivalDate = date;

            if (oState == dState)
            {



                totalDuration = originTime + destTime;
                arrivalDate = date.AddHours(totalDuration);
            }

            // each routes duration is split in half for crossing states
            // Interstate travel costs are then charged at a flat rate of each states weekday off peak rate
            // because these costs are fixed they have been calculated outside of the program and added to each scenario below

            /* e.g.
                FR1_MelbSyd = 9;	4.5 * $244 + 4.5 * $245 = 
                FR2_MelbHob = 22;	11 * $244 + 11 * $214 = $5038
                FR2_MelbHob + FR1_MelbSyd = $5038 + $2200.5 = $7238.50
             */
            Double TravelCost = 0;

            switch (oState)
            {
                case "TAS":
                    switch (dState)
                    {
                        case "VIC":
                            totalDuration = originTime + FR2_MelbHob + destTime; TravelCost += 5038;                       
                            break;
                        case "NSW":
                            totalDuration = originTime + FR2_MelbHob + FR1_MelbSyd + destTime; TravelCost += 7238.5;
                            break;
                        case "QLD":
                            totalDuration = originTime + FR2_MelbHob + FR1_MelbSyd + FR5_SydBris + destTime; TravelCost += 9588.5;
                            break;
                        case "ACT":
                            totalDuration = originTime + FR2_MelbHob + FR1_MelbSyd + FR4_SydCan + destTime; TravelCost += 7981;
                            break;
                        case "SA":
                            totalDuration = (originTime + FR2_MelbHob + FR3_MelbAdel + destTime) - ACST; TravelCost += 7094; // applies timezone logic
                            break;
                        case "NT":
                            totalDuration = (originTime + FR2_MelbHob + FR3_MelbAdel + FR9_PerthAdel + destTime) - AEST; TravelCost += 14024; // applies timezone logic
                            break;
                        case "WA":
                            totalDuration = (originTime + FR2_MelbHob + FR3_MelbAdel + FR8_AdelDar + destTime) - ACST; TravelCost += 14969; // applies timezone logic
                            break;           
                    }
                    goto default;
                              
                case "VIC":
                    switch (dState)
                    {
                        case "TAS":
                            totalDuration = originTime + FR2_MelbHob + destTime; TravelCost += 5038;
                            break;
                        case "NSW":
                            totalDuration = originTime + FR1_MelbSyd + destTime; TravelCost += 2200.5;
                            break;
                        case "QLD":
                            totalDuration = originTime + FR1_MelbSyd + FR5_SydBris + destTime; TravelCost += 4550.5;
                            break;
                        case "ACT":
                            totalDuration = originTime + FR1_MelbSyd + FR4_SydCan + destTime; TravelCost += 2943;
                            break;
                        case "SA":
                            totalDuration = (originTime + FR3_MelbAdel + destTime) - ACST; TravelCost += 2056; // applies timezone logic
                            break;
                        case "NT":
                            totalDuration = (originTime + FR3_MelbAdel + FR8_AdelDar + destTime) - ACST; TravelCost += 9931; // applies timezone logic
                            break;
                        case "WA":
                            totalDuration = (originTime + FR3_MelbAdel + FR9_PerthAdel + destTime) - AEST; TravelCost += 8986; // applies timezone logic                            
                            break;
                    }
                    goto default;

                case "NSW":
                    switch (dState)
                    {
                        case "TAS":
                            totalDuration = originTime + FR1_MelbSyd + FR2_MelbHob + destTime; TravelCost += 7238.5;
                            break;
                        case "VIC":
                            totalDuration = originTime + FR1_MelbSyd + destTime; TravelCost += 2200.5;
                            break;
                        case "QLD":
                            totalDuration = originTime + FR5_SydBris + destTime; TravelCost += 2350;
                            break;
                        case "ACT":
                            totalDuration = originTime + FR4_SydCan + destTime; TravelCost += 742.5;
                            break;
                        case "SA":
                            totalDuration = (originTime + FR1_MelbSyd + FR3_MelbAdel + destTime) - ACST; TravelCost += 4256.5; // applies timezone logic
                            break;
                        case "NT":
                            totalDuration = (originTime + FR5_SydBris + FR6_BrisDar + destTime) - ACST; TravelCost += 10450; // applies timezone logic
                            break;
                        case "WA":
                            totalDuration = (originTime + FR1_MelbSyd + FR3_MelbAdel + FR9_PerthAdel + destTime) - AEST; TravelCost += 11186.5; // applies timezone logic 
                            break;
                    }
                    goto default;

                case "QLD":
                    switch (dState)
                    {
                        case "TAS":
                            totalDuration = originTime + FR5_SydBris + FR1_MelbSyd + FR2_MelbHob + destTime; TravelCost += 9588.5;
                            break;
                        case "VIC":
                            totalDuration = originTime + FR5_SydBris + FR1_MelbSyd + destTime; TravelCost += 4550.5;
                            break;
                        case "NSW":
                            totalDuration = originTime + FR5_SydBris + destTime; TravelCost += 2350;
                            break;
                        case "ACT":
                            totalDuration = originTime + FR5_SydBris + FR4_SydCan + destTime; TravelCost += 3092.5;
                            break;
                        case "SA":
                            totalDuration = (originTime + FR5_SydBris + FR1_MelbSyd + FR3_MelbAdel + destTime) - ACST; TravelCost += 6606.5;// applies timezone logic
                            break;
                        case "NT":
                            totalDuration = (originTime + FR6_BrisDar + destTime) - ACST; TravelCost += 8100; // applies timezone logic
                            break;
                        case "WA":
                            totalDuration = (originTime + FR5_SydBris + FR1_MelbSyd + FR3_MelbAdel + FR9_PerthAdel + destTime) - AEST; TravelCost += 13536.5; // applies timezone logic
                            break;
                    }
                    goto default;
                case "ACT":
                    switch (dState)
                    {
                        case "TAS":
                            totalDuration = originTime + FR4_SydCan + FR1_MelbSyd + FR2_MelbHob + destTime; TravelCost += 7981;
                            break;
                        case "VIC":
                            totalDuration = originTime + FR4_SydCan + FR1_MelbSyd + destTime; TravelCost += 2943;
                            break;
                        case "NSW":
                            totalDuration = originTime + FR4_SydCan + destTime; TravelCost += 742.5;
                            break;
                        case "QLD":
                            totalDuration = originTime + FR4_SydCan + FR5_SydBris + destTime; TravelCost += 3092.5;
                            break;
                        case "SA":
                            totalDuration = (originTime + FR4_SydCan + FR1_MelbSyd + FR3_MelbAdel + destTime) - ACST; TravelCost += 4999; // applies timezone logic
                            break;
                        case "NT":
                            totalDuration = (originTime + FR4_SydCan + FR5_SydBris + FR6_BrisDar + destTime) - ACST; TravelCost += 11192.5; // applies timezone logic 
                            break;
                        case "WA":
                            totalDuration = (originTime + FR4_SydCan + FR1_MelbSyd + FR3_MelbAdel + FR9_PerthAdel + destTime) - AEST; TravelCost += 11929; // applies timezone logic
                            break;
                    }
                    goto default;

                case "SA":
                    switch (dState)
                    {
                        case "TAS":
                            totalDuration = (originTime + FR3_MelbAdel + FR2_MelbHob + destTime) + ACST; TravelCost += 7094;  // applies timezone logic           
                            break;
                        case "VIC":
                            totalDuration = (originTime + FR3_MelbAdel + destTime) + ACST; TravelCost += 2056; // applies timezone logic
                            break;
                        case "NSW":
                            totalDuration = (originTime + FR3_MelbAdel + FR1_MelbSyd + destTime) + ACST; TravelCost += 4256.5; // applies timezone logic
                            break;
                        case "QLD":
                            totalDuration = (originTime + FR3_MelbAdel + FR1_MelbSyd + FR5_SydBris + destTime) + ACST; TravelCost += 6606.5; // applies timezone logic; 
                            break;
                        case "ACT":
                            totalDuration = originTime + FR3_MelbAdel + FR1_MelbSyd + FR4_SydCan + destTime + ACST; TravelCost += 4999; // applies timezone logic
                            break;
                        case "NT":
                            totalDuration = originTime + FR8_AdelDar + destTime; TravelCost += 7875;
                            break;
                        case "WA":
                            totalDuration = (originTime + FR9_PerthAdel + destTime) - AWST; TravelCost += 6930; // applies timezone logic
                            break;
                    }
                    goto default;
                case "NT":
                    switch (dState)
                    {
                        case "TAS":
                            totalDuration = (originTime + FR8_AdelDar + FR3_MelbAdel + FR2_MelbHob + destTime) + ACST; TravelCost += 14024; // applies timezone logic
                            break;
                        case "VIC":
                            totalDuration = (originTime + FR8_AdelDar + FR3_MelbAdel + destTime) + ACST; TravelCost += 9931; // applies timezone logic
                            break;
                        case "NSW":
                            totalDuration = (originTime + FR6_BrisDar + FR5_SydBris + destTime) + ACST; TravelCost += 10450; // applies timezone logic
                            break;
                        case "QLD":
                            totalDuration = (originTime + FR6_BrisDar + destTime) + ACST; TravelCost += 8100; // applies timezone logic
                            break;
                        case "ACT":
                            totalDuration = (originTime + FR6_BrisDar + FR5_SydBris + FR4_SydCan + destTime) + ACST; TravelCost += 11192.5; // applies timezone logic
                            break;
                        case "SA":
                            totalDuration = originTime + FR8_AdelDar + destTime; TravelCost += 7875;
                            break;
                        case "WA":
                            totalDuration = (originTime + FR7_DarPerth + destTime) - AWST; TravelCost += 8707.5; // applies timezone logic
                            break;
                    }
                    goto default;
                case "WA":
                    switch (dState)
                    {
                        case "TAS":
                            totalDuration = (originTime + FR9_PerthAdel + FR3_MelbAdel + FR2_MelbHob + destTime) + AEST; TravelCost += 14969; // applies timezone logic
                            break;
                        case "VIC":
                            totalDuration = (originTime + FR9_PerthAdel + FR3_MelbAdel + destTime) + AEST; TravelCost += 8986; // applies timezone logic
                            break;
                        case "NSW":
                            totalDuration = (originTime + FR9_PerthAdel + FR3_MelbAdel + FR1_MelbSyd + destTime) + AEST; TravelCost += 11186.5; // applies timezone logic
                            break;
                        case "QLD":
                            totalDuration = (originTime + FR7_DarPerth + FR6_BrisDar + destTime) + AEST; TravelCost += 13536.5; // applies timezone logic
                            break;
                        case "ACT":
                            totalDuration = (originTime + FR9_PerthAdel + FR3_MelbAdel + FR1_MelbSyd + FR4_SydCan + destTime) + AEST; TravelCost += 11929; // applies timezone logic
                            break;
                        case "SA":
                            totalDuration = (originTime + FR9_PerthAdel + destTime) + AWST; TravelCost += 6930; // applies timezone logic
                            break;
                        case "NT":
                            totalDuration = (originTime + FR7_DarPerth + destTime) + AWST;  TravelCost += 8707.5; // applies timezone logic
                            break;
                    }
                    goto default;

                default:
                    arrivalDate = date.AddHours(totalDuration);
                    break;

            }
            #endregion
            #endregion
            //
            // Travel Cost Logic
            //
            #region travel cost
            DateTime startTime = date;
            DateTime endTime = startTime.AddHours(originTime);
            
            // declare 2 rates for on/off peak
            double rate1 = 0;
            double rate2 = 0;

            // assign rates based on calendar logic
            if (IsPublicHoliday(date, oState))
            {
                rate1 = rates[4];
                rate2 = rates[5];
            }
            else
                if (IsDayOfWeekBetween(date, DayOfWeek.Monday, DayOfWeek.Friday))
                {
                    rate1 = rates[0];
                    rate2 = rates[1];
                }
                else
                {
                    rate1 = rates[2];
                    rate2 = rates[3];
                }

            // calculate cost from assigned rates by checking cross referencing start/end times with on/off peak times
            if ((IsTimeOfDayBetween(startTime, onpeak, offpeak)) && (IsTimeOfDayBetween(endTime, offpeak, onpeak)))
            {
                double beforePeak = Convert.ToDouble(offpeak.Hours - startTime.TimeOfDay.Hours);
                TravelCost += beforePeak * rate1;
                double afterPeak = Convert.ToDouble(endTime.TimeOfDay.Hours - offpeak.Hours);
                TravelCost += afterPeak * rate2;
            }
            else
                if ((IsTimeOfDayBetween(startTime, offpeak, onpeak)) && (IsTimeOfDayBetween(endTime, onpeak, offpeak)))
                {
                    double beforePeak = Convert.ToDouble(offpeak.Hours - startTime.TimeOfDay.Hours);
                    TravelCost += beforePeak * rate2;
                    double afterPeak = Convert.ToDouble(endTime.TimeOfDay.Hours - offpeak.Hours);
                    TravelCost += afterPeak * rate1;
                }
                else
                    if ((IsTimeOfDayBetween(startTime, onpeak, offpeak)) && (IsTimeOfDayBetween(endTime, onpeak, offpeak)))
                    {
                        TravelCost += originTime * rate1;
                    }
                    else
                        TravelCost += originTime * rate2;
               
            // again for desitination    
            startTime = endTime;
            endTime = startTime.AddHours(destTime);

            if (IsPublicHoliday(arrivalDate, dState))
            {
                rate1 = rates[4];
                rate2 = rates[5];
            }
            else
                if (IsDayOfWeekBetween(date, DayOfWeek.Monday, DayOfWeek.Friday))
                {
                    rate1 = rates[0];
                    rate2 = rates[1];
                }
                else
                {
                    rate1 = rates[2];
                    rate2 = rates[3];
                }

            // calculate cost from assigned rates by checking cross referencing start/end times with on/off peak times
            if ((IsTimeOfDayBetween(startTime, onpeak, offpeak)) && (IsTimeOfDayBetween(endTime, offpeak, onpeak)))
            {
                double beforePeak = Convert.ToDouble(offpeak.Hours - startTime.TimeOfDay.Hours);
                TravelCost += beforePeak * rate1;
                double afterPeak = Convert.ToDouble(endTime.TimeOfDay.Hours - offpeak.Hours);
                TravelCost += afterPeak * rate2;
            }
            else
                if ((IsTimeOfDayBetween(startTime, offpeak, onpeak)) && (IsTimeOfDayBetween(endTime, onpeak, offpeak)))
                {
                    double beforePeak = Convert.ToDouble(offpeak.Hours - startTime.TimeOfDay.Hours);
                    TravelCost += beforePeak * rate2;
                    double afterPeak = Convert.ToDouble(endTime.TimeOfDay.Hours - offpeak.Hours);
                    TravelCost += afterPeak * rate1;
                }
                else
                    if ((IsTimeOfDayBetween(startTime, onpeak, offpeak)) && (IsTimeOfDayBetween(endTime, onpeak, offpeak)))
                    {
                        TravelCost += destTime * rate1;
                    }
                    else
                        TravelCost += destTime * rate2;
            #endregion
            //
            // Price calculation logic
            //
            #region Message Queue
            connectionFactory = new ConnectionFactory(BROKER, CLIENT_ID);
            connection = connectionFactory.CreateConnection();
            connection.Start();
            session = connection.CreateSession();
            subscriber = new Subscriber(session, TOPIC_NAME);
            subscriber.Start(CONSUMER_ID);
            subscriber.OnMessageReceived += new MessageReceivedDelegate(subscriber_OnMessageReceived);

            using (var publisher = new Publisher(session, TOPIC_NAME))
            {
                publisher.SendMessage(TravelCost.ToString());
            }

            Thread.Sleep(1000);
            try
            {
                subscriber.Dispose();
                session.Close();
                session.Dispose();
                connection.Stop();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }

            #endregion
            //
            // Rebate Processor
            //
            #region Rebate Processor
            // Fetch membership class
            conn.Open();
            SqlDataReader rdr = null;
            cmd = new SqlCommand("SELECT * FROM membership WHERE MembershipId=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", txtMemberId.Text);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            string memclass = rdr.GetString(3);
            conn.Close();

            // Obtain order status
            conn.Open();
            rdr = null;
            cmd = new SqlCommand("SELECT * FROM booking WHERE MemberId=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", txtMemberId.Text);
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                OrderExists = true;
            }
            conn.Close();

            // Process rebate
            double i = 0;
            double discount = 0;
            double totaldiscount = 0;
            switch (memclass)
            {
                case "Regular":
                    if (OrderExists)
                    {
                        price = price - 300;
                        totaldiscount = totaldiscount + 300; 
                    }
                    goto default;

                case "Silver":
                    while (price - i > 5000)
                    {
                        i = i + 5000;
                        discount = price * 0.10;
                        totaldiscount += discount;
                        price = price - discount;

                    }
                    goto default;

                case "Gold":
                    if (OrderExists)
                    {
                        price = price - 300;
                    }
                    
                    while (price - i > 5000)
                    {
                        i = i + 5000;
                        discount = price * 0.15;
                        totaldiscount += discount;
                        price = price - discount;

                    }
                    totaldiscount = totaldiscount + 300;
                    goto default;

                default:
                    break;
            }

            // Apply GST
            Double gst = (price * 0.10); // apply GST
            Double totalprice = price + gst;
            #endregion
            //
            // Booking Processor
            //
            #region Save booking

            // Bindings for placeholders
            var orderId = Math.Abs(DateTime.Now.GetHashCode());
            var orderDate = DateTime.Now.ToShortDateString();
            var details =
                // costs (3)
                gst.ToString("c") + "," + TravelCost.ToString("c") + "," + totaldiscount.ToString("c") + "," +
                // origin (5)
                txtFullName1.Text + "," + txtAddress1.Text + "," + txtCity1.Text + "," + txtState1.Text + "," + txtPostCode1.Text + "," +
                // destination (5)
                txtFullName2.Text + "," + txtAddress2.Text + "," + txtCity2.Text + "," + txtState2.Text + "," + txtPostCode2.Text + "," +
                // billing (6)
                txtFullName3.Text + "," + txtPhone.Text + "," + txtAddress3.Text + "," + txtCity3.Text + "," + txtState3.Text + "," + txtPostCode3.Text + "," +
                // order items (4)
                txtTrucks.Text + "," + date.ToString() + "," + arrivalDate.ToString() + "," + totalDuration.ToString();

            // Begin database operation
            conn.Open();
            transaction = conn.BeginTransaction("OrderTransaction");
            cmd.Connection = conn;
            cmd.Transaction = transaction;
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@MemberId", txtMemberId.Text);
            cmd.Parameters.AddWithValue("@OrderDate", DateTime.Parse(orderDate));
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@OrderDetails", details);

            try
            {
                cmd.CommandText =
                    "INSERT INTO BOOKING (OrderId,MemberId,OrderDate,Price,OrderDetails) VALUES (@OrderId,@MemberId,@OrderDate,@Price,@OrderDetails)";
                cmd.ExecuteNonQuery();

                transaction.Commit();
                lblError.Text = "Order added successfully to database.";
            }
                catch (Exception ex)
                {
                    lblMember.Text = ex.Message;

                    // Attempt to roll back the transaction. 
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        lblMember.Text = ex2.Message;
                    }
                }
            

            conn.Close();

            #endregion
            //
            // Begin pdf document generation
            //
            #region pdf generation
            var document = new Document();
            HTMLWorker htmlworker = new HTMLWorker(document);
            var pdfoutput = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, pdfoutput);
            document.Open();
            string contents = File.ReadAllText(Server.MapPath("receipt.htm"));

            // Placeholders for Order receipt
            var origin = txtFullName1.Text + "<br />" + txtAddress1.Text + "<br />" + txtCity1.Text + "<br />" + txtState1.Text + " " + txtPostCode1.Text;
            var destination = txtFullName2.Text + "<br />" + txtAddress2.Text + "<br />" + txtCity2.Text + "<br />" + txtState2.Text + " " + txtPostCode2.Text;
            var billing = txtFullName3.Text + "<br />" + txtPhone.Text + "<br />" + txtAddress3.Text + "<br />" + txtCity3.Text + "<br />" + txtState3.Text + " " + txtPostCode3.Text;
            contents = contents.Replace("[ORDERID]", orderId.ToString());
            contents = contents.Replace("[ORDERDATE]", orderDate);
            contents = contents.Replace("[TOTALCHARGES]", totalprice.ToString("c"));
            contents = contents.Replace("[TOTALGST]", gst.ToString("c"));
            contents = contents.Replace("[TRAVELCOSTS]", TravelCost.ToString("c"));
            contents = contents.Replace("[DISCOUNT]", totaldiscount.ToString("c"));
            contents = contents.Replace("[BILLING]", billing);
            contents = contents.Replace("[ORIGIN]", origin);
            contents = contents.Replace("[DESTINATION]", destination);
            contents = contents.Replace("[TRUCKS]", txtTrucks.Text);
            contents = contents.Replace("[DELDATE]", date.ToString());
            contents = contents.Replace("[ARRDATE]", arrivalDate.ToString());
            contents = contents.Replace("[TOTALHOURS]", totalDuration.ToString());

            var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(contents), null);

            foreach (var htmlElement in parsedHtmlElements)
                document.Add(htmlElement as IElement);

            document.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename=Receipt-{0}.pdf", orderId)); 
            Response.BinaryWrite(pdfoutput.ToArray());
            Response.End();
            #endregion
        }           
    }
    // Check membership
    protected void btnMember(Object Sender, EventArgs E)
    {

        SqlConnection conn = null;
        SqlDataReader rdr = null;

        lblMember.Text = "";
        conn = new SqlConnection(
            @"Data Source=.\SQLEXPRESS; AttachDbFilename='|DataDirectory|\employees.mdf';
            Integrated Security=True; User Instance=True");

        try
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM membership WHERE MembershipId=@Id", conn);

            // Search Parameters
            cmd.Parameters.AddWithValue("@Id", txtMemberId.Text);

            rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                txtFullName1.Text = rdr["FirstName"].ToString() + " " + rdr["LastName"].ToString();
                txtFullName3.Text = rdr["FirstName"].ToString() + " " + rdr["LastName"].ToString();
                MemberExists = true;
                
            }
            else
                lblMember.Text = "Member not found";
            
        }
        finally
        {
            if (rdr != null)
            { rdr.Close(); }
        }
        if (conn != null)
        {
            conn.Close();
        }

    }
    // Price calculation on MQ
    void subscriber_OnMessageReceived(string message)
    {
        double _travelcost = Convert.ToDouble(message);
        double trucks = Convert.ToDouble(txtTrucks.Text);
        price = (trucks * 1000); // apply flat rates for each truck
        price += trucks * _travelcost; // apply travel cost to each truck
    }
}

