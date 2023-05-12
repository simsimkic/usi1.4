using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.Users;
public abstract class User: Serializable, INotifyPropertyChanged
{
    private uint _id;
    private string _firstName;
    private string _lastName;
    private string _username;
    private string _password;

    public uint Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged("Id"); }
    }

    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged("Username"); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged("Password"); }
    }
    
    public string FirstName
    {
        get => _firstName;
        set { _firstName = value; OnPropertyChanged("FirstName"); }
    }

    public string LastName
    {
        get => _lastName;
        set { _lastName = value; OnPropertyChanged("LastName"); }
    }

    public string FullName => String.Concat(FirstName, " ", LastName);

    protected User()
    {
        _id = 0;
        _username = "";
        _password = "";
        _firstName = "";
        _lastName = "";
    }
    
    protected User(uint id)
    {
        _id = 0;
        _username = "";
        _password = "";
        _firstName = "";
        _lastName = "";
    }

    protected User(uint id, string username, string password, string firstName, string lastName)
    {
        _id = id;
        _username = username;
        _password = password;
        _firstName = firstName;
        _lastName = lastName;
    }

    public static User FindUser(List<User> collection, string username, string password)
    {
        foreach (var user in collection)
        {
            if (user.Username == username && user.Password == password)
                return user;
        }
        return null;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
    public abstract string[] ToCSV();
    public abstract void FromCSV(string[] values);
}