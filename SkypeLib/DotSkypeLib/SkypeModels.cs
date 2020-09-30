using System;
using System.Collections.Generic;
using System.Text;

namespace SkypeLib
{
    public class ResponseImageId
    {
        public string id { get; set; }
    }
    public class Location
    {
        public string type { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string state { get; set; }
    }
    public class Name
    {
        public string first { get; set; }
        public string company { get; set; }
        public string surname { get; set; }
    }
    public class Profile
    {
        public string avatar_url { get; set; }
        public string gender { get; set; }
        public List<Location> locations { get; set; }
        public Name name { get; set; }
        public string about { get; set; }
        public string skype_handle { get; set; }
        public string language { get; set; }
        public string website { get; set; }
    }
    public class Source
    {
        public string type { get; set; }
        public string subtype { get; set; }
        public DateTime time { get; set; }
    }

    public class RelationshipHistory
    {
        public List<Source> sources { get; set; }
    }

    public class Info
    {
        public List<string> capabilities { get; set; }
        public string trusted { get; set; }
        public string type { get; set; }
    }

    public class Agent
    {
        public List<string> capabilities { get; set; }
        public string trust { get; set; }
        public string type { get; set; }
        public Info info { get; set; }
    }

    public class Contact
    {
        public string person_id { get; set; }
        public string mri { get; set; }
        public string display_name { get; set; }
        public string display_name_source { get; set; }
        public Profile profile { get; set; }
        public bool authorized { get; set; }
        public bool blocked { get; set; }
        public bool @explicit { get; set; }
        public DateTime creation_time { get; set; }
        public RelationshipHistory relationship_history { get; set; }
        public Agent agent { get; set; }
    }

    public class ContactInfo
    {
        public List<Contact> contacts { get; set; }
        public string scope { get; set; }
        public int count { get; set; }
    }

    public class UserProfile
    {
        public object about { get; set; }
        public string avatarUrl { get; set; }
        public object birthday { get; set; }
        public object city { get; set; }
        public object country { get; set; }
        public List<string> emails { get; set; }
        public string firstname { get; set; }
        public string gender { get; set; }
        public object homepage { get; set; }
        public object jobtitle { get; set; }
        public object language { get; set; }
        public object lastname { get; set; }
        public object mood { get; set; }
        public object phoneHome { get; set; }
        public string phoneMobile { get; set; }
        public object phoneOffice { get; set; }
        public object province { get; set; }
        public object richMood { get; set; }
        public string username { get; set; }
    }



    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Metadata
    {
        public int totalCount { get; set; }
        public string forwardLink { get; set; }
        public string backwardLink { get; set; }
        public string syncState { get; set; }
    }

    public class ThreadProperties
    {
        public string lastjoinat { get; set; }
        public string members { get; set; }
        public string topic { get; set; }
        public string membercount { get; set; }
        public string lastleaveat { get; set; }
        public string version { get; set; }
    }

    public class Properties
    {
        public string consumptionhorizonpublished { get; set; }
        public string isemptyconversation { get; set; }
        public string consumptionhorizon { get; set; }
        public string isfollowed { get; set; }
        public DateTime lastimreceivedtime { get; set; }
    }

    public class LastMessage
    {
        public string from { get; set; }
        public string type { get; set; }
        public string conversationLink { get; set; }
    }

    public class ConversactionItem
    {
        public string targetLink { get; set; }
        public ThreadProperties threadProperties { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public long version { get; set; }
        public Properties properties { get; set; }
        public LastMessage lastMessage { get; set; }
        public string messages { get; set; }
    }

    public class ConversactionThread
    {
        public Metadata _metadata { get; set; }
        public List<ConversactionItem> conversations { get; set; }
    }
}
