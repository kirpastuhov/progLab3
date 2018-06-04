namespace progLab3
{
    using Enumerable = System.Linq.Enumerable;

    public class TaskRunner
    {
        private readonly Menu _mainMenu;

        private readonly System.Text.RegularExpressions.Regex _isbnRegex = new System.Text.RegularExpressions.Regex(@"(?=[-0-9xX ]{13}$)(?:[0-9]+[- ]){3}[0-9]*[xX0-9]$");
        private const string Url = "http://127.0.0.1:8000/connection";
        private readonly System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient();
        [System.Runtime.Serialization.DataMember]
        private System.Collections.Generic.List<Books> _books = new System.Collections.Generic.List<Books>();

        public TaskRunner()
        {
            _mainMenu = new Menu();

            _mainMenu.Items.AddRange(new System.Collections.Generic.List<MenuItem>
            {
                new MenuItem
                {
                    Label = "Post",
                    Key = '1',
                    Function = P
                },
                new MenuItem
                {
                    Label = "Get",
                    Key = '2',
                    Function = G
                },
                new MenuItem
                {
                    Label = "add",
                    Key = '3',
                    Function = AddBook
                },

                new MenuItem
                {
                    Label = "Library",
                    Key = '4',
                    Function = ListBooks
                },
                new MenuItem
                {
                    Label = "SaveFromINILibrary",
                    Key = '5',
                    Function = SaveLibFromIni
                },
                new MenuItem
                {
                    Label = "WriteToINI",
                    Key = '6',
                    Function = WriteBookToIni
                },
                new MenuItem
                {
                    Label = "SaveFromJSONLibrary",
                    Key = '7',
                    Function = ReadLibraryFromJson
                },


                new MenuItem
                {
                    Label = "WriteToJSON",
                    Key = '8',
                    Function = WriteBookToJson
                },
                new MenuItem
                {
                    Label = "Quit",
                    Key = '0',
                    Function = () => false
                }

            });
        }

        public void Run()
        {
            _mainMenu.Display();
        }

        private bool P()
        {
            Post();
            return true;
        }

        private bool G()
        {
            try
            {
                var a = Get();
                a.Wait();
            }
            catch (System.AggregateException aggregateException)
            {
                System.Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("\nServer is not responding");
                System.Console.WriteLine("Message :{0} ", aggregateException.Message);
            }

            return true;
        }

        private async System.Threading.Tasks.Task Get()
        {
            try
            {
                var responseString = await _client.GetStringAsync(Url);
                //Console.WriteLine($"Response : {responseString.GetType()}");
                System.Console.WriteLine($"Response : {responseString}");
                WriteStrToJ(responseString);
                ReadLibraryFromJson();

            }
            catch (System.Net.Http.HttpRequestException exception)
            {
                System.Console.WriteLine("\nException Caught!");	
                System.Console.WriteLine("Message : {0} ",exception.Message);
            }
            catch (System.Threading.Tasks.TaskCanceledException exception)
            {
                System.Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("Message :{0} ", exception.Message);
            }
            catch (System.AggregateException aggregateException)
            {
                System.Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("Message :{0} ", aggregateException.Message);
            }
        }

        private static void WriteStrToJ(string text)
        {
            System.IO.File.WriteAllText("RemoteLibrary.json", text);
           
        }

        private async void Post()
        {
            try
            {
                var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(_books);
                var responseMessageesponse = await _client.GetAsync(Url);
                responseMessageesponse.EnsureSuccessStatusCode();
                var body = System.Text.Encoding.UTF8.GetBytes(json);
                var request = (System.Net.HttpWebRequest) System.Net.WebRequest.Create(Url);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = body.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }

                using (var response = (System.Net.HttpWebResponse) request.GetResponse())
                {
                    System.Console.WriteLine($"Status Code: {response.StatusCode}");
                    System.Console.WriteLine($"Method : {response.Method}");

                    response.Close();
                }
            }
            catch (System.AggregateException aggregateException)
            {
                System.Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("Message :{0} ", aggregateException.Message);
            }
            catch (System.ObjectDisposedException disposedException)
            {
                System.Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("Message :{0} ", disposedException.Message);
            }
            catch (System.Threading.Tasks.TaskCanceledException exception)
            {
                System.Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("Message :{0} ", exception.Message);
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                System.Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (System.Net.WebException webException)
            {
                System.Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("Message :{0} ", webException.Message);
            }
            
            _client.Dispose();
        }


        private bool WriteBookToJson()
        {
       
            var bookArray = _books.ToArray();
            var jsonFormatter = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Books[]));
            if (bookArray.Length == 0)
                System.Console.WriteLine("Library is empty");
            else
            {
                using (var fs = new System.IO.FileStream("RemoteLibrary.json", System.IO.FileMode.OpenOrCreate))
                {
                    jsonFormatter.WriteObject(fs, bookArray);
                }
            }

            return true;

        }

        private bool ReadLibraryFromJson()
        {
            var jsonFormatter = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Books[]));
            using (var fs = new System.IO.FileStream("Library.json", System.IO.FileMode.OpenOrCreate))
            {
                if (fs.Length == 0)
                {
                    System.Console.WriteLine("JSON is empty");
                }
                else
                {


                    var newBooks = (Books[]) jsonFormatter.ReadObject(fs);
                    var lst = Enumerable.ToList(Enumerable.OfType<Books>(newBooks));
                    System.Console.WriteLine("Done reading from json");
                    foreach (var b in lst)
                    {
                        var book = new Books
                        {
                            Author = b.Author,
                            Title = b.Title,
                            Annotation = b.Annotation,
                            Isbn = b.Isbn,
                            PublicationDate = b.PublicationDate
                        };

                        _books.Add(book);
                    }
                }
            }

            return true;
        }



        private bool SaveLibFromIni()
        {
            var parser = new IniParser.FileIniDataParser();
            var data = parser.ReadFile("Lib2.ini");

            foreach (var section in data.Sections)
            {
                var book = new Books
                {
                    Isbn = section.SectionName,
                    Title = data[section.SectionName]["Title"],
                    Author = data[section.SectionName]["Author"],
                    PublicationDate = data[section.SectionName]["PublicationDate"],
                    Annotation = data[section.SectionName]["Annotation"]
                };
                _books.Add(book);
            }
            return true;
        }


        private bool WriteBookToIni()
        {
            var parser = new IniParser.FileIniDataParser();
            var data = parser.ReadFile("Library.ini");

            foreach (var book in _books)
            {
                //Add a new selection and some keys
                data.Sections.AddSection(book.Isbn);
                data[book.Isbn].AddKey("Author", book.Author);
                data[book.Isbn].AddKey("Title", book.Title);
                //data[book.ISBN].AddKey("PublicationDate", book.PublicationDate.ToString("d"));
                data[book.Isbn].AddKey("PublicationDate", book.PublicationDate);
                data[book.Isbn].AddKey("Annotation", book.Annotation);

                parser.WriteFile("Library.ini", data);
            }

            return true;
        }


        private bool ListBooks()
        {

            foreach (var book in _books)
                System.Console.WriteLine($"{book}");

            return true;
        }

        private bool AddBook()
        {
            var book = new Books();
            string input;

            do
            {
                System.Console.Write("Input title: ");
                input = System.Console.ReadLine()?.Trim();
            } while (string.IsNullOrWhiteSpace(input));

            book.Title = input;

            do
            {
                System.Console.Write("Input author: ");
                input = System.Console.ReadLine()?.Trim();
            } while (string.IsNullOrWhiteSpace(input));

            book.Author = input;

            do
            {
                System.Console.Write("Input annotation: ");
                input = System.Console.ReadLine()?.Trim();
            } while (string.IsNullOrWhiteSpace(input));

            book.Annotation = input;

            do
            {
                System.Console.Write("Input ISBN(1-333-55555-1): ");
                input = System.Console.ReadLine()?.Trim();
            } while (!_isbnRegex.IsMatch(input ?? throw new System.InvalidOperationException()));

            book.Isbn = input;

            if (_books.Find(b => string.Compare(b.Isbn, book.Isbn, System.StringComparison.OrdinalIgnoreCase) == 0) != null)
            {
                System.Console.WriteLine("There's a book with the same ISBN in library.");
                return true;
            }

            System.DateTime date;

            do
            {
                System.Console.Write("Input publication date (mm-dd-yyyy): ");
                input = System.Console.ReadLine()?.Trim();
            } while (!System.DateTime.TryParse(input, out date));

            book.PublicationDate = $"{date:d}";
            _books.Add(book);

            return true;
        }
    }
}
