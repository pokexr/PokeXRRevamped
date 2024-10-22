using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONData : MonoBehaviour
{
}

[Serializable]
public class VideosList
{
    public List<Video> videos;
}

[Serializable]
public class Video
{
    public int id ;
    public string name ;
    public string original_video_name ;
    public DateTime created_at ;
    public DateTime updated_at ;
    public string status ;
    public string result_video_url ;
    public string placeholder ;
    public string thumbnail_url ;
    public bool featured ;
    public int user_id ;
    public string user_name;
}



[Serializable]
public class Meta
{
    public string token;
}

[Serializable]
public class Root
{
    public User user;
    public Meta meta;
}

[Serializable]
public class User
{
    public int id;
    public string name;
    public string username;
    public string email;
    public object color;
    public string gender;
    public string birthday;
    public object package;
    public bool enable_counter;
    public int allowed_videos;
    public bool is_read;
    public int followees_count;
    public string quick_blox_id;
    public string image_url;
}
namespace FriendList
{
    [Serializable]
    public class FriendsList
    {
        public List<User> users;
    }
    [Serializable]
    public class User
    {
        public int id;
        public string username;
        public string name;
        public string color;
        public string image_url;
        public string email;
        public string chat_id;
    }
}

[Serializable]
public class GeneralResponceRoot
{
    public bool success;
    public string msg;
}


namespace UpdatedUserInfo
{
    [Serializable]
    public class Root
    {
        public User user;
    }
    [Serializable]
    public class User
    {
        public int id;
        public string name;
        public string username;
        public string email;
        public string color;
        public string gender;
        public string birthday;
        public object package;
        public bool enable_counter;
        public int allowed_videos;
        public bool is_read;
        public int followees_count;
        public string quick_blox_id;
        public string image_url;
    }
}

[Serializable]
public class GetUserVideosRoot
{
    public bool success;
    public List<UserVideo> user_videos;
}
[Serializable]
public class UserVideo
{
    public int id;
    public string unscreen_id;
    public string status;
    public string result_url;
    public int user_id;
    public DateTime created_at;
    public DateTime updated_at;
    public string result_video_url;
    public string name;
    public string placeholder;
    public string thumbnail_url;
    public bool featured;
}

[Serializable]
public class Datum
{
    public int id;
    public int sender_id;
    public int receiver_id;
    public string message;
    public DateTime created_at;
    public DateTime updated_at;
}
// Chat Class
[Serializable]
public class RetrieveRoot
{
    public bool success;
    public List<Datum> data;
}
[Serializable]
public class GeneralResponce
{
    public bool success;
    public string msg;
}
namespace Chat
{
    [Serializable]
    public class User
    {
        public int id;
        public string username;
        public string name;
        public string color;
        public string image_url;
        public string email;
        public string chat_id;
    }
    [Serializable]
    public class FriendsList
    {
        public List<User> users;
    }
}


namespace SendDeviceID
{
    [Serializable]
    public class Root
    {
        public User user;
    }
    [Serializable]
    public class User
    {
        public int id;
        public string name;
        public string username;
        public string email;
        public string color;
        public string gender;
        public DateTime birthday;
        public object package;
        public bool enable_counter;
        public int allowed_videos;
        public bool is_read;
        public int followees_count;
        public string device_registration_id;
        public string image_url;
    }
}

[Serializable]
public class NumberData
{
    public List<int> numbers;
}