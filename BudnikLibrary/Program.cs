using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace BudnikLibrary
{
	class Program
	{
		static void Main(string[] args)
		{
			new ViewClass(new ControllerClass()).Show();
		}
	}

	public class ModelClass
	{
		public class IID
		{
			public int ItemId;
		}

		public class Author : IID
		{
			public string Name;
			public string Sorname;
			public List<int> BooksWritten = new List<int>();
		}

		public class Book : IID
		{
			public int Author;
			public string Name;
			public int Year;
			public List<int> BookCopies = new List<int>();
		}

		public class Copy : IID
		{
			public string ID;
			public int Book;
			public int OnHand;
			public DateTime DateTaken;
		}

		public class Reader : IID
		{
			public string Name;
			public string Sorname;
			public string Adress;
			public int TelNumber;
			public List<int> OnHand = new List<int>();
		}

		public List<Author> Authors = new List<Author>();
		public List<Book> Books = new List<Book>();
		public List<Copy> Copies = new List<Copy>();
		public List<Reader> Readers = new List<Reader>();
	}

	public class ViewClass
	{
		protected ControllerClass controller;

		public ViewClass(ControllerClass controller)
		{
			this.controller = controller;
		}

		public void Show()
		{
			while (true)
			{
				ModelClass.IID o = null;
				switch (ReadChoice(true, "Главное меню", new string[]{
				"Выдача","Возврат","Поиск","Добавить","Удалить","Загрузить","Сохранить","Выход"
				}))
				{
					case 0:
						var copy = Search("Выберите экземпляр для выдачи. Введите id, артикул экземпляра или название книги",
							controller.FindCopies, DisplayCopyShort);
						if (copy == null) break;
						if (copy.OnHand != 0)
						{
							Console.WriteLine("Книга уже выданa читателю " + DisplayReaderShort(controller.GetReader(copy.OnHand)));
							Console.ReadKey();
							break;
						}
						var reader = Search("Выберите читателя. Введите id, Имя Фамилию, номер телефона или адрес читателя ",
							controller.FindReaders, DisplayReaderShort);
						if (reader == null) break;
						controller.RentBook(copy.ItemId, reader.ItemId);
						Console.WriteLine("Книга выдана");
						Console.ReadKey();
						break;
					case 1:
						var reader2 = Search("Выберите читателя. Введите id, Имя Фамилию, номер телефона или адрес читателя ",
							controller.FindReaders, DisplayReaderShort);
						if (reader2 == null) break;
						if (reader2.OnHand.Count == 0)
						{
							Console.WriteLine("Нету книг для возврата");
							Console.ReadKey();
							break;
						}
						int retcopyid = ReadChoice(false, "Выберите книгу для возврата", new string[]{
						"Выйти" }.Concat(reader2.OnHand.Select(x => DisplayCopyShort(controller.GetCopy(x)))).ToArray());
						switch (retcopyid)
						{
							case 0: break;
							default:
								retcopyid = reader2.OnHand[retcopyid - 1];
								var retcopy = controller.GetCopy(retcopyid);
								int days = (int)DateTime.Now.Subtract(retcopy.DateTaken).TotalDays;
								if (days > 30)
									Console.WriteLine("Возврат задержан, сумма штрафа: "
										+ (days - 30).ToString() + " x 4.5 = " + (days * 4.5F).ToString());
								else Console.WriteLine("Книга возвращена");
								controller.ReturnBook(retcopyid);
								Console.ReadKey();
								break;
						}
						break;
					case 2:
						o = SearchMenu("Поиск");
						if (o != null)
						{
							Console.WriteLine();
							Console.WriteLine("Нажмите любую кнопку чтобы вернутся в главное меню");
							Console.ReadKey();
						}
						break;
					case 3:
						switch (ReadChoice(true, "Добавить", new string[]{
								"Автора","Книгу","Экземпляр книги","Читателя","Вернутся в главное меню"
						}))
						{
							case 0:
								o = controller.AddAuthor(ReadString(false, "Введите имя"), ReadString(false, "Введите фамилию"));
								if (o != null) Console.WriteLine("\r\n" + DisplayAuthor(o as ModelClass.Author));
								break;
							case 1:
								var a = Search("Выберите автора. Введите id или Имя Фамилию автора", controller.FindAuthors, DisplayAuthorShort);
								if (a == null) break;
								o = controller.AddBook(a.ItemId, ReadString(false, "Введите название"), ReadInt(false, "Введите год издания"));
								if (o != null) Console.WriteLine("\r\n" + DisplayBook(o as ModelClass.Book));
								break;
							case 2:
								var b = Search("Введите id, название книги, год или автора", controller.FindBooks, DisplayBookShort);
								if (b == null) break;
								o = controller.AddCopy(b.ItemId, ReadString(false, "Введите артикул"));
								if (o != null) Console.WriteLine("\r\n" + DisplayCopy(o as ModelClass.Copy));
								break;
							case 3:
								o = controller.AddReader(ReadString(false, "Введите Имя"), ReadString(true, "Введите Фамилию"),
									ReadString(false, "Введите адресс"), ReadInt(false, "Введите телефон"));
								if (o != null) Console.WriteLine("\r\n" + DisplayReader(o as ModelClass.Reader));
								break;
						}
						if (o != null)
						{
							Console.WriteLine();
							Console.WriteLine("Добавлено. Нажмите любую кнопку чтобы вернутся в главное меню");
							Console.ReadKey();
						}
						break;
					case 4:
						o = SearchMenu("Удалить");
						if (o != null)
						{
							Console.WriteLine();
							if (ReadChoice(false, "Удалить этот обьект?", new string[] { "Нет", "Да" }) == 1)
							{
								if (o is ModelClass.Author) controller.RemoveAuthor(o.ItemId);
								else if (o is ModelClass.Book) controller.RemoveBook(o.ItemId);
								else if (o is ModelClass.Copy) controller.RemoveCopy(o.ItemId);
								else if (o is ModelClass.Reader) controller.RemoveReader(o.ItemId);
							}
						}
						break;
					case 5:
						controller.LoadModel("data.json");
						Console.Clear();
						Console.WriteLine();
						Console.WriteLine("Загружено");
						Console.ReadKey();
						break;
					case 6:

						controller.SaveModel("data.json");
						Console.Clear();
						Console.WriteLine();
						Console.WriteLine("Сохранено");
						Console.ReadKey();
						break;
					case 7:
						return;
				}
			}
		}

		protected string ReadString(bool clear, string title)
		{
			if (clear) Console.Clear();
			Console.WriteLine();
			Console.WriteLine(title + ": ");
			Console.WriteLine();
			return Console.ReadLine();
		}

		protected int ReadInt(bool clear, string title)
		{
			if (clear) Console.Clear();
			Console.WriteLine();
			Console.WriteLine(title + ": ");
			Console.WriteLine();
			string s = Console.ReadLine();
			int res;
			while (!int.TryParse(s, out res))
			{
				if (clear) Console.Clear();
				Console.WriteLine();
				Console.WriteLine(title + ": ");
				Console.WriteLine();
				s = Console.ReadLine();
			}
			return res;
		}

		protected int ReadChoice(bool clear, string title, string[] items)
		{
			while (true)
			{
				if (clear)
					Console.Clear();
				Console.WriteLine();
				Console.Write(title + ": ");
				int left = Console.CursorLeft;
				int top = Console.CursorTop;
				Console.WriteLine();
				Console.WriteLine();
				for (int i = 0; i < items.Length; i++)
				{
					Console.WriteLine(i.ToString() + ". " + items[i]);
				}
				int left2 = Console.CursorLeft;
				int top2 = Console.CursorTop;
				Console.SetCursorPosition(left, top);
				//}
				//else
				//{
				//	Console.WriteLine();
				//	for (int i = 0; i < items.Length; i++)
				//	{
				//		Console.WriteLine(i.ToString() + ". " + items[i]);
				//	}
				//	Console.WriteLine();
				//	Console.Write(title + ": ");
				//	Console.WriteLine();
				//}
				int ans;
				if (int.TryParse(Console.ReadLine(), out ans) && ans >= 0 && ans < items.Length)
				{
					Console.SetCursorPosition(left2, top2);
					return ans;
				}
				Console.SetCursorPosition(left2, top2);
			}
		}

		protected ModelClass.IID SearchMenu(string title)
		{
			ModelClass.IID result = null;
			switch (ReadChoice(true, title, new string[]{
								"Автора","Книгу","Экземпляр книги","Читателя","Вернутся в главное меню"
						}))
			{
				case 0:
					result = Search("Введите id или Имя Фамилию автора", controller.FindAuthors, DisplayAuthorShort);
					if (result != null) Console.WriteLine("\r\nАвтор\r\n" + DisplayAuthor(result as ModelClass.Author));
					return result;
				case 1:
					result = Search("Введите id, название книги, год или автора", controller.FindBooks, DisplayBookShort);
					if (result != null) Console.WriteLine("\r\nКнига\r\n" + DisplayBook(result as ModelClass.Book));
					return result;
				case 2:
					result = Search("Введите id, артикул экземпляра или название книги", controller.FindCopies, DisplayCopyShort);
					if (result != null) Console.WriteLine("\r\nЭкземпляр книги\r\n" + DisplayCopy(result as ModelClass.Copy));
					return result;
				case 3:
					result = Search("Введите id, Имя Фамилию, номер телефона или адрес читателя ", controller.FindReaders, DisplayReaderShort);
					if (result != null) Console.WriteLine("\r\nЧитатель\r\n" + DisplayReader(result as ModelClass.Reader));
					return result;
			}
			return null;
		}

		protected T Search<T>(string title, Func<string, T[]> finder, Func<T, string> display) where T : class
		{
			while (true)
			{
				string s = ReadString(true, title);
				var res = finder(s);
				if (res.Length == 0)
				{
					switch (ReadChoice(false, "Ничего не найдено. Повторить поиск?", new string[]{
						"Нет", "Да"	}))
					{
						case 0: return null;
					}
				}
				else if (res.Length == 1)
				{
					Console.WriteLine();
					Console.Write(display(res[0]));
					Console.WriteLine();
					switch (ReadChoice(false, "Вы это искали?", new string[]{
						"Нет", "Да"	, "Повторить поиск" }))
					{
						case 0: return null;
						case 1: return res[0];
					}
				}
				else
				{
					int choice = ReadChoice(false, "Много совпадений. Выберите нужное", new string[]{
						"Повторить поиск", "Выйти" }.Concat(res.Select(x => display(x))).ToArray());
					switch (choice)
					{
						case 0: break;
						case 1: return null;
						default: return res[choice - 2];
					}

				}
			}
		}

		protected string DisplayAuthor(ModelClass.Author author)
		{
			var s = new StringBuilder();
			s.AppendLine("id: " + author.ItemId.ToString());
			s.AppendLine("Имя: " + author.Name);
			s.AppendLine("Фамилия: " + author.Sorname);
			s.AppendLine("Написанные книги (" + author.BooksWritten.Count.ToString() + "): ");
			foreach (var bookid in author.BooksWritten)
			{
				var book = controller.GetBook(bookid);
				s.AppendLine(book.Year.ToString() + " " + book.Name);
			}
			return s.ToString();
		}
		protected string DisplayReader(ModelClass.Reader reader)
		{
			var s = new StringBuilder();
			s.AppendLine("id: " + reader.ItemId.ToString());
			s.AppendLine("Имя: " + reader.Name);
			s.AppendLine("Фамилия: " + reader.Sorname);
			s.AppendLine("Телефон: " + reader.TelNumber);
			s.AppendLine("Адресс: " + reader.Adress);
			s.AppendLine("Взятые книги (" + reader.OnHand.Count.ToString() + "): ");
			foreach (var copyid in reader.OnHand)
			{
				var copy = controller.GetCopy(copyid);
				s.AppendLine(copy.DateTaken.ToString() + " " + DisplayBookShort(controller.GetBook(copy.Book)));
			}
			return s.ToString();
		}
		protected string DisplayBook(ModelClass.Book book)
		{
			var s = new StringBuilder();
			s.AppendLine("id: " + book.ItemId.ToString());
			s.AppendLine("Название: " + book.Name);
			s.AppendLine("Год: " + book.Year.ToString());
			s.AppendLine("Автор: " + DisplayAuthorShort(controller.GetAuthor(book.Author)));
			s.AppendLine("Экземпляры (" + book.BookCopies.Count.ToString() + "): ");
			foreach (var copyid in book.BookCopies)
			{
				s.AppendLine(DisplayCopyShort(controller.GetCopy(copyid)));
			}
			return s.ToString();
		}
		protected string DisplayCopy(ModelClass.Copy copy)
		{
			var s = new StringBuilder();
			s.AppendLine("id: " + copy.ItemId.ToString());
			s.AppendLine("Артикул: " + copy.ID.ToString());
			s.AppendLine("Книга: " + DisplayBookShort(controller.GetBook(copy.Book)));
			if (copy.OnHand != 0)
			{
				s.AppendLine("На руках у: " + DisplayReaderShort(controller.GetReader(copy.OnHand)));
				s.AppendLine("Взята: " + copy.DateTaken.ToString());
			}
			else
				s.AppendLine("Книга в библиотеке");
			return s.ToString();
		}

		protected string DisplayAuthorShort(ModelClass.Author author)
		{
			return author.Name + " " + author.Sorname;
		}
		protected string DisplayReaderShort(ModelClass.Reader reader)
		{
			return reader.Name + " " + reader.Sorname;
		}
		protected string DisplayBookShort(ModelClass.Book book)
		{
			return book.Name + " " + book.Year.ToString();
		}
		protected string DisplayCopyShort(ModelClass.Copy copy)
		{
			return copy.ID.ToString() + " " + controller.GetBook(copy.Book).Name;
		}
	}

	public class ControllerClass
	{
		protected static Random Random = new Random();
		protected static int GenId(IEnumerable<ModelClass.IID> existingIDs)
		{
			int newId = Random.Next();
			while (existingIDs.Any(x => x.ItemId == newId))
				newId = Random.Next();
			return newId;
		}

		public T[] FindWithScore<T>(string s, IList<T> items, Func<T, string, bool> score)
		{

			var keys = s.ToLower().Split(new char[] { ',', ' ' });

			return items.Select(x =>
				new KeyValuePair<int, T>(keys.Count(k => score(x, k)), x))
					.OrderByDescending(x => x.Key).TakeWhile(x => x.Key > 0).Select(x => x.Value).ToArray();
		}

		public ModelClass.Author[] FindAuthors(string s)
		{
			return FindWithScore(s, Model.Authors, (x, k) =>
				x.Name.ToLower().IndexOf(k) >= 0 || x.Sorname.ToLower().IndexOf(k) >= 0
				|| x.ItemId.ToString().IndexOf(k) >= 0);
		}
		public ModelClass.Book[] FindBooks(string s)
		{
			return FindWithScore(s, Model.Books, (x, k) =>
				x.Name.ToLower().IndexOf(k) >= 0 || x.Year.ToString() == k
				|| GetAuthor(x.Author).Name.ToLower().IndexOf(k) >= 0
				|| GetAuthor(x.Author).Sorname.ToLower().IndexOf(k) >= 0
				|| x.ItemId.ToString().IndexOf(k) >= 0);

		}
		public ModelClass.Reader[] FindReaders(string s)
		{
			return FindWithScore(s, Model.Readers, (x, k) =>
			x.Name.ToLower().IndexOf(k) >= 0 || x.Sorname.ToLower().IndexOf(k) >= 0
			|| x.TelNumber.ToString().ToLower().IndexOf(k) >= 0
			|| x.Adress.ToLower().IndexOf(k) >= 0
			|| x.ItemId.ToString().IndexOf(k) >= 0);
		}
		public ModelClass.Copy[] FindCopies(string s)
		{
			return FindWithScore(s, Model.Copies, (x, k) =>
				GetBook(x.Book).Name.IndexOf(k) >= 0
				|| GetBook(x.Book).Year.ToString() == k
				|| x.ID.ToLower().IndexOf(k) >= 0
				|| x.DateTaken.ToString().IndexOf(k) >= 0
				|| x.ItemId.ToString().IndexOf(k) >= 0);
		}

		public ModelClass.Author GetAuthor(int id)
		{
			return Model.Authors.Find(x => x.ItemId == id);
		}
		public ModelClass.Book GetBook(int id)
		{
			return Model.Books.Find(x => x.ItemId == id);

		}
		public ModelClass.Reader GetReader(int id)
		{
			return Model.Readers.Find(x => x.ItemId == id);

		}
		public ModelClass.Copy GetCopy(int id)
		{
			return Model.Copies.Find(x => x.ItemId == id);

		}

        /// <summary>
        /// Create new author
        /// </summary>
        /// <param name="name">Name of new author</param>
        /// <param name="sorname">Sorname of new author</param>
        /// <returns>created author</returns>
		public ModelClass.Author AddAuthor(string name, string sorname)
		{
			var newAuthor = new ModelClass.Author()
			{
				ItemId = GenId(Model.Authors),
				Name = name,
				Sorname = sorname
			};
			Model.Authors.Add(newAuthor);
			return newAuthor;
		}
        /// <summary>
        /// Create a new book
        /// </summary>
        /// <param name="author"> Autor's name of book</param>
        /// <param name="name">name of book</param>
        /// <param name="year">Year when the book was written</param>
        /// <returns>created book</returns>
		public ModelClass.Book AddBook(int author, string name, int year)
		{
			var newBook = new ModelClass.Book()
			{
				ItemId = GenId(Model.Books),
				Author = author,
				Name = name,
				Year = year
			};
			Model.Books.Add(newBook);
			GetAuthor(author).BooksWritten.Add(newBook.ItemId);
			return newBook;
		}
		public ModelClass.Reader AddReader(string name, string sorname, string adress, int telNumber)
		{
			var newReader = new ModelClass.Reader()
			{
				ItemId = GenId(Model.Readers),
				Name = name,
				Sorname = sorname,
				Adress = adress,
				TelNumber = telNumber
			};
			Model.Readers.Add(newReader);
			return newReader;
		}
		public ModelClass.Copy AddCopy(int book, string id)
		{
			var newCopy = new ModelClass.Copy()
			{
				ItemId = GenId(Model.Readers),
				ID = id,
				Book = book,
				DateTaken = DateTime.Now
			};
			Model.Copies.Add(newCopy);
			GetBook(book).BookCopies.Add(newCopy.ItemId);
			return newCopy;
		}

		public void RemoveAuthor(int id)
		{
			var author = GetAuthor(id);
			if (author != null)
			{
				foreach (var bookid in author.BooksWritten)
				{
					var book = GetBook(id);
					if (book != null)
					{
						foreach (var copyid in book.BookCopies)
						{
							var copy = GetCopy(copyid);
							if (copy != null)
							{
								var reader = GetReader(copy.OnHand);
								if (reader != null) reader.OnHand.Remove(id);

								Model.Copies.Remove(copy);
							}
						}
					}
					Model.Books.Remove(book);
				}

				Model.Authors.Remove(author);
			}
		}
		public void RemoveBook(int id)
		{
			var book = GetBook(id);
			if (book != null)
			{
				foreach (var copyid in book.BookCopies)
				{
					var copy = GetCopy(copyid);
					if (copy != null)
					{
						var reader = GetReader(copy.OnHand);
						if (reader != null) reader.OnHand.Remove(id);

						Model.Copies.Remove(copy);
					}
				}

				var author = GetAuthor(book.Author);
				if (author != null) author.BooksWritten.Remove(id);
			}
			Model.Books.Remove(book);
		}
		public void RemoveReader(int id)
		{
			var reader = GetReader(id);
			if (reader != null)
			{
				foreach (var copyid in reader.OnHand)
				{
					var copy = GetCopy(id);
					if (copy != null)
					{
						copy.OnHand = 0;
					}
				}

				Model.Readers.Remove(reader);
			}
		}
		public void RemoveCopy(int id)
		{
			var copy = GetCopy(id);
			if (copy != null)
			{
				var book = GetBook(copy.Book);
				if (book != null) book.BookCopies.Remove(id);

				var reader = GetReader(copy.OnHand);
				if (reader != null) reader.OnHand.Remove(id);

				Model.Copies.Remove(copy);
			}
		}

		public void RentBook(int copyid, int readerid)
		{
			var copy = GetCopy(copyid);
			if (copy == null) return;

			var reader = GetReader(readerid);
			if (reader == null) return;

			copy.DateTaken = DateTime.Now;
			copy.OnHand = readerid;

			reader.OnHand.Add(copyid);
		}
		public void ReturnBook(int copyid)
		{
			var copy = GetCopy(copyid);
			if (copy == null) return;

			//var reader = GetReader(copy.ItemId);
			//if (reader == null) return;

			copy.OnHand = 0;

			//reader.OnHand.Remove(copyid);
		}

		public void SaveModel(string Path)
		{
			using (var F = new System.IO.FileStream(Path, System.IO.FileMode.Create))
				new DataContractJsonSerializer(typeof(ModelClass)).WriteObject(F, Model);
		}
		public void LoadModel(string Path)
		{
			if (System.IO.File.Exists(Path))
				using (var F = new System.IO.FileStream(Path, System.IO.FileMode.Open))
					Model = (ModelClass)new DataContractJsonSerializer(typeof(ModelClass)).ReadObject(F);
		}

		protected ModelClass Model = new ModelClass();
	}
}
