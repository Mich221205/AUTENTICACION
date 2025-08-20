using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

[DataContract]
public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; }
    [DataMember] public string identificacion { get; set; }
    [DataMember] public string nombre { get; set; }
    [DataMember] public string apellido1 { get; set; }
    [DataMember] public string apellido2 { get; set; }
    [DataMember] public string correo { get; set; }
    [DataMember] public string usuario { get; set; }
    [DataMember] public string contrasenna { get; set; }
    [DataMember] public string estado { get; set; }
    [DataMember] public int tipo { get; set; }
}
