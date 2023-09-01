#if false
using MySteez;

namespace UnitTests
{
	public class KeepSortingStruct
	{
		[Fact]
		public void KeepSorting_CorrectUse_Class()
		{
			List<entry> list = new();
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 5444 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 35 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 51, Name = "me" });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 5444 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 4 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 4 });
			list.Add(new entry { Order = 3333334 });
			list.Add(new entry { Order = -233 });

			list.Sort();
			CheckSorted(list);

			// move me 10 places forward
			KeepSorted.Mutate(list, (curr, prev) =>
			{
				if (curr.Name == "me")
				{
					curr.Order += 10;
					return true;
				}
				return false;
			});

			CheckSorted(list);
		}

		private void CheckSorted(List<entry> list)
		{
			entry? prev = null;
			foreach (var ele in list)
			{
				if (prev != null)
				{
					var c = ele.CompareTo(prev.Value);
					if (c >= 0)
					{
						// ok
					}
					else
					{
						throw new Exception("not sorted");
					}
				}

				prev = ele;
			}
		}




		struct entry : IComparable<entry>
		{
			public string Name;
			public int Order;

			public int CompareTo(entry other)
			{
				return this.Order.CompareTo(other.Order);
			}

			public override string ToString()
			{
				return $"Order {Order} Name {Name}";
			}
		}

		[Fact]
		public void KeepSorting_CorrectUse_Class2()
		{
			List<entry> list = new();
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 5444 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 35 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 51, Name = "me" });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 61 });
			list.Add(new entry { Order = 5444 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 4 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 4 });
			list.Add(new entry { Order = 3333334 });
			list.Add(new entry { Order = -233 });

			list.Sort();
			CheckSorted(list);

			// move me 10 places forward
			KeepSorted.Mutate(list, (curr, prev) =>
			{
				if (curr.Name == "me")
				{
					curr.Order += 10;
					return true;
				}
				return false;
			});

			CheckSorted(list);

		}

		[Fact]
		public void KeepSorting_CorrectUse_Class3()
		{
			List<entry> list = new();
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 35 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 51, Name = "me" });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 35 });
			list.Add(new entry { Order = 54 });


			list.Sort();
			CheckSorted(list);

			// move me 10 places forward
			KeepSorted.Mutate(list, (curr, prev) =>
			{
				if (curr.Name == "me")
				{
					curr.Order += 10;
					return true;
				}
				return false;
			});

			CheckSorted(list);

		}



		[Fact]
		public void KeepSorting_CorrectUse_Class4()
		{
			List<entry> list = new();
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 35 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 61, Name = "me" });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 35 });
			list.Add(new entry { Order = 54 });


			list.Sort();
			CheckSorted(list);

			// move me 10 places forward
			KeepSorted.Mutate(list, (curr, prev) =>
			{
				if (curr.Name == "me")
				{
					curr.Order += 10;
					return true;
				}
				return false;
			});

			CheckSorted(list);

		}

		[Fact]
		public void KeepSorting_CorrectUse_Class5()
		{
			List<entry> list = new();
			list.Add(new entry { Order = 61, Name = "me" });


			list.Sort();
			CheckSorted(list);

			// move me 10 places forward
			KeepSorted.Mutate(list, (curr, prev) =>
			{
				if (curr.Name == "me")
				{
					curr.Order += 10;
					return true;
				}
				return false;
			});

			CheckSorted(list);

		}



		[Fact]
		public void KeepSorting_CorrectUse_Class6()
		{
			List<entry> list = new();
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 61, Name = "me" });


			list.Sort();
			CheckSorted(list);

			// move me 10 places forward
			KeepSorted.Mutate(list, (curr, prev) =>
			{
				if (curr.Name == "me")
				{
					curr.Order += 10;
					return true;
				}
				return false;
			});

			CheckSorted(list);

		}

		[Fact]
		public void KeepSorting_CorrectUse_Class7()
		{
			List<entry> list = new();
			list.Add(new entry { Order = 61, Name = "me" });
			list.Add(new entry { Order = 1 });

			list.Sort();
			CheckSorted(list);

			// move me 10 places forward
			KeepSorted.Mutate(list, (curr, prev) =>
			{
				if (curr.Name == "me")
				{
					curr.Order += 10;
					return true;
				}
				return false;
			});

			CheckSorted(list);

		}

		[Fact]
		public void KeepSorting_WrongUse_Struct()
		{
			List<entry> list = new();
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 5 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 5444 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 35 });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 51, Name = "me" });
			list.Add(new entry { Order = 54 });
			list.Add(new entry { Order = 5444 });
			list.Add(new entry { Order = -2 });
			list.Add(new entry { Order = 4 });
			list.Add(new entry { Order = 1 });
			list.Add(new entry { Order = 4 });
			list.Add(new entry { Order = 3333334 });
			list.Add(new entry { Order = -233 });

			list.Sort();
			CheckSorted(list);

			Assert.Throws<Exception>(() =>
			{
				// move me 10 places forward
				KeepSorted.Mutate(list, (curr, prev) =>
				{
					if (curr.Name == "me")
					{
						curr.Order -= 10;
						return true;
					}
					return false;
				});
			});

			//Assert.Throws<Exception>(() => CheckSorted(list));
		}

	}
}

#endif