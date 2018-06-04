using System.Text;
using System.Runtime.Serialization;


namespace progLab3
{

    [DataContract]
    public class Books
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Annotation { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string Isbn { get; set; }

        //public DateTime PublicationDate;
        [DataMember]
        public string PublicationDate { get; set; }


        private string ToString(bool annotation = true)
        {

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Title: ").Append(Title).Append('\n');
            stringBuilder.Append("Author: ").Append(Author).Append('\n');
            stringBuilder.Append("ISBN: ").Append(Isbn).Append('\n');
            //stringBuilder.Append("PublicationDate: ").Append(PublicationDate.ToString("d-m-yyyy")).Append('\n');
            stringBuilder.Append("PublicationDate:").Append(PublicationDate).Append('\n');

            if (annotation)
            {
                stringBuilder.Append("Annotation: ").Append(Annotation).Append('\n');
            }

            return stringBuilder.ToString();
        }

        public override string ToString() => ToString();
    }
}
