#define WRITE_OA

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using NotVisualBasic;
using NotVisualBasic.FileIO;
using static Pivot.Tester;

// Tidbits.RowGasp
// group, agg, sort, pivot. Gasp.
// Tidbits.PivotDataTable <- this!
namespace Pivot
{
	//	Region,              Country,Item Type,   Sales Channel,Order Priority, Order Date,Order ID, Ship Date,Units Sold, Unit Price,Unit Cost, Total Revenue,Total Cost, Total Profit
	//Australia and Oceania, Palau, Office Supplies,Online,     H,              3/6/2016,517073523,  3/26/2016,2401,651.21,524.96,1563555.21,1260428.96,303126.25
	public class CsvRow
	{
		[Index(0)]
		public string Region { get; set; }
		[Index(1)]
		public string Country { get; set; }
		[Index(2)]
		public string ItemType { get; set; }
		[Index(3)]
		public string SalesChannel { get; set; }
		[Index(4)]
		public string OrderPriority { get; set; }
		[Index(5)]
		public DateTime OrderDate { get; set; }
		[Index(6)]
		public string OrderID { get; set; }
		[Index(7)]
		public DateTime ShipDate { get; set; }
		[Index(8)]
		public long UnitsSold { get; set; }
		[Index(9)]
		public double UnitPrice { get; set; }
		[Index(10)]
		public double UnitCost { get; set; }
		[Index(11)]
		public double TotalRevenue { get; set; }
		[Index(12)]
		public double TotalCost { get; set; }
		[Index(13)]
		public double TotalProfit { get; set; }
		
	}

	public class Table
	{
		public List<TableColumn> Columns { get; set; }
		public List<object?[]> Rows { get; set; }
		public object?[]? GrandTotalRow { get; set; }
	}

	public class TableColumn
	{
		public string Name { get; set; }
		public object DataType { get; set; }

		public FieldType FieldType { get; set; }
		public int GroupIndex { get; set; }

		public Sorting Sorting { get; set; }
		public int SortIndex { get; set; }







		public TableGroup[]? ColumnGroups { get; set; }
	}

	//public class TableRow
	//{
	//	public object?[] Values { get; set; }
	//}

	//public class CsvRowDataFetcher<TRow> : ITypedList
	//{
	//	PropertyDescriptorCollection _props;

	//	public CsvRowDataFetcher()
 //       {
	//		_props = TypeDescriptor.GetProperties(typeof(TRow));
	//	}
	//	public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
	//	{
	//		return _props;
	//	}

	//	public string GetListName(PropertyDescriptor[] listAccessors)
	//	{
	//		return "";
	//	}
	//}

	public class Tester
	{
		public static void Main()
		{
			Version v = null;

			string fff = "" + v;

			var t = new Tester();
			t.Test();
		}

		public void Test()
		{
			//var datas = new CsvTextFieldParser(@"d:\5m Sales Records.csv");

			//while (!datas.EndOfData)
			//{
			//	var fields = datas.ReadFields();
			//}

			List<CsvRow> allRTows = null;

			using (var reader = new StreamReader(@"d:\5m Sales Records.csv"))
			using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				var records = csv.GetRecords<CsvRow>();

				allRTows = records.ToList();
			}

			//var props = TypeDescriptor.GetProperties(typeof(CsvRow));

			var fieldsss = CreateFieldsFromType<CsvRow>();// (props);



			// TODO: Skulle kanskje hatt en måte å sette indeks på likevel? Som var uavhengig av rekkefølgen i Ienumerable?
	//		MoveToTop(fieldsss, "OrderDate");
//			MoveToTop(fieldsss, "ItemType");
			//MoveToTop(fieldsss, "OrderID");
			//MoveToTop(fieldsss, "ItemType");

			GetField(fieldsss, "Region").FieldType = FieldType.RowGroup;
			GetField(fieldsss, "Region").GroupIndex = 1;
			GetField(fieldsss, "Region").Sorting = Sorting.Asc;
			GetField(fieldsss, "Region").SortIndex = 1;

			GetField(fieldsss, "Country").FieldType = FieldType.RowGroup;
			GetField(fieldsss, "Country").GroupIndex = 2;
			GetField(fieldsss, "Country").Sorting = Sorting.Asc;
			GetField(fieldsss, "Country").SortIndex = 2;

			GetField(fieldsss, "SalesChannel").FieldType = FieldType.ColGroup;
			GetField(fieldsss, "SalesChannel").Sorting = Sorting.Asc;
			GetField(fieldsss, "SalesChannel").SortIndex = 1;
			GetField(fieldsss, "SalesChannel").GroupIndex = 3;

			GetField(fieldsss, "ItemType").FieldType = FieldType.ColGroup;
			GetField(fieldsss, "ItemType").Sorting = Sorting.Asc;
			GetField(fieldsss, "ItemType").SortIndex = 0;
			GetField(fieldsss, "ItemType").GroupIndex = 0;

			//GetField(fieldsss, "Country").Area = Area.Group;
			//GetField(fieldsss, "Country").Sort = Sort.Asc;

			//GetField(fieldsss, "ShipDate").Sort = Sort.Desc;

			fieldsss.Add(new Field { FieldType = FieldType.Measure, FieldName = "RowCount", Sorting = Sorting.Desc, DataType = typeof(int), SortIndex = 0 });

			var props = new List<PropertyDescriptor>();

			props.Add(new ColumnFunc<CsvRow, string>(nameof(CsvRow.Region), rows => GetCommaList(rows, row => row.Region)));
			props.Add(new ColumnFunc<CsvRow, string>(nameof(CsvRow.Country), rows => GetCommaList(rows, row => row.Country)));
			props.Add(new ColumnFunc<CsvRow, string>(nameof(CsvRow.ItemType), rows => GetCommaList(rows, row => row.ItemType)));
			props.Add(new ColumnFunc<CsvRow, string>(nameof(CsvRow.SalesChannel), rows => GetCommaList(rows, row => row.SalesChannel)));
			props.Add(new ColumnFunc<CsvRow, string>(nameof(CsvRow.OrderPriority), rows => GetCommaList(rows, row => row.OrderPriority)));

			props.Add(new ColumnFunc<CsvRow, DateTime>(nameof(CsvRow.OrderDate), rows => rows.Max(r => r.OrderDate)));

			props.Add(new ColumnFunc<CsvRow, string>(nameof(CsvRow.OrderID), rows => GetCommaList(rows, row => row.OrderID)));
			props.Add(new ColumnFunc<CsvRow, int>("RowCount", rows => rows.Count()));

			props.Add(new ColumnFunc<CsvRow, DateTime>(nameof(CsvRow.ShipDate), rows => rows.Max(r => r.ShipDate)));

		
			props.Add(new ColumnFunc<CsvRow, long>(nameof(CsvRow.UnitsSold), rows => rows.Sum(r => r.UnitsSold)));


			props.Add(new ColumnFunc<CsvRow, double>(nameof(CsvRow.UnitPrice), rows => rows.Sum(r => r.UnitPrice)));


			props.Add(new ColumnFunc<CsvRow, double>(nameof(CsvRow.UnitCost), rows => rows.Sum(r => r.UnitCost)));


			props.Add(new ColumnFunc<CsvRow, double>(nameof(CsvRow.TotalRevenue), rows => rows.Sum(r => r.TotalRevenue)));


			props.Add(new ColumnFunc<CsvRow, double>(nameof(CsvRow.TotalCost), rows => rows.Sum(r => r.TotalCost)));

			props.Add(new ColumnFunc<CsvRow, double>(nameof(CsvRow.TotalProfit), rows => rows.Sum(r => r.TotalProfit)));

			var sw = Stopwatch.StartNew();

//			TypeValue: object, name, fullname

			var pp = new Pivoter<CsvRow>(fieldsss, allRTows, new PropertyDescriptorCollection(props.ToArray()));
			//var dt = pp.GetTable();
			var dt = pp.GetTableIntersect();
			sw.Stop();

			//dt.WriteXml(@"d:\testdt5mill.xml");
			using (var f = File.OpenWrite(@"d:\testdt5mill2.json"))
			{
				JsonSerializer.Serialize(f, dt, new JsonSerializerOptions { WriteIndented=true});
			}

			// 37 sek uten DT eller object arrays
			// 35 sek med DT??
			// 186 rows

			// 58sec,  3.6GB, Count = 2097153
			// DT: 2min,  5.4GB, Count = 2097153

			var pdc = new PropertyDescriptorCollection(new PropertyDescriptor[]
			{
				new SiteNameCol(),
				new UnitNameCol(),
				new SpeciesNameCol().Col,
				new IndCountCol().Col
			});
			
			var list = new Rows<Row>(pdc);

			// TODO: optin: RowNumber auto column? 1 to n?
			// TODO: optin: top row fieldnames? (or caption?)

			list.Add(new Row() { IndCount = 11, SiteName = "S1", UnitName = "U1", SpecName = "Frog"});
			list.Add(new Row() { IndCount = 13, SiteName = "S1", UnitName = "U1", SpecName = "Hog" });
			list.Add(new Row() { IndCount = 22, SiteName = "S1", UnitName = "U2", SpecName = "Bird" });
			list.Add(new Row() { IndCount = 33, SiteName = "S2", UnitName = "U1", SpecName = "Dog" });
			list.Add(new Row() { IndCount = 44, SiteName = "S3", UnitName = "U1", SpecName = "Human" });
			list.Add(new Row() { IndCount = 111, SiteName = "S1", UnitName = "U11", SpecName = "Frog" });
			list.Add(new Row() { IndCount = 222, SiteName = "S1", UnitName = "U22", SpecName = "Bird" });
			list.Add(new Row() { IndCount = 333, SiteName = "S2", UnitName = "U11", SpecName = "Dog" });
			list.Add(new Row() { IndCount = 444, SiteName = "S3", UnitName = "U11", SpecName = "Human" });


			var siteF = new FieldGen<string>() { FieldType = FieldType.Measure, FieldName = "SiteName", Sorting = Sorting.Asc, SortIndex = 0 };

			

//			var unitF = new FieldGen<string>() { Area = Area.Group, FieldName = "UnitName"  };
			var specF = new FieldGen<string>() { FieldType = FieldType.Measure, FieldName = "SpeciesName", Sorting = Sorting.Desc, SortIndex = 1 };
			var indF = new FieldGen<int>() { FieldType = FieldType.Measure, FieldName = "IndCount" };
			var ff = new Field[] { specF,   indF, siteF };

			var p = new Pivoter<Row>(ff, list, new PropertyDescriptorCollection(props.ToArray()));
			p.GetTable();
			// TODO: dt can be slow? add option to use different construct? and then need different sorting?
		}

		private string GetCommaList(IEnumerable<CsvRow> rows, Func<CsvRow, string> value)
		{
			int constrainedCount = rows.Take(2).Count();
			if (constrainedCount == 0)
				return "";
			else if (constrainedCount == 1)
				return value(rows.Single());
			else
				return string.Join(", ", rows.Select(value).Distinct().OrderBy(v => v));
		}

		private List<Field> CreateFieldsFromType<T>()
		{
			return typeof(T).GetProperties().Select(pd => new Field { FieldType = FieldType.Measure, FieldName = pd.Name, DataType = pd.PropertyType }).ToList();
		}

		private Field GetField(List<Field> fieldsss, string v)
		{
			return fieldsss.Where(f => f.FieldName == v).Single();
		}

		private void MoveToTop(List<Field> fieldsss, string field)
		{
			var sing = fieldsss.Where(f => f.FieldName == field).Single();
			fieldsss.Remove(sing);
			fieldsss.Insert(0, sing);
		}

		private List<Field> CreateFieldsFromProps(PropertyDescriptorCollection props)
		{
			return props.Cast<PropertyDescriptor>().Select(pd => new Field { FieldType = FieldType.Measure, FieldName = pd.Name, DataType = pd.PropertyType }).ToList();
		}

		public class SiteNameCol : Column<Row, string>
		{
            public SiteNameCol() : base("SiteName")
            {
                
            }

			protected override string GetRowValue(IEnumerable<Row> rows)
			{
				return string.Join(", ", rows.Select(r => r.SiteName).Distinct().OrderBy(s => s));
			}
		}

		public class UnitNameCol : Column<Row, string>
		{
			public UnitNameCol() : base("UnitName")
			{

			}

			protected override string GetRowValue(IEnumerable<Row> rows)
			{
				return string.Join(", ", rows.Select(r => r.UnitName).Distinct().OrderBy(s => s));
			}
		}

		public class SpeciesNameCol
		{
			public ColumnFunc<Row, string> Col;

			public SpeciesNameCol()
			{
				Col = new ColumnFunc<Row, string>("SpeciesName", GetRowValue);
			}

			string GetRowValue(IEnumerable<Row> rows)
			{
				return string.Join(", ", rows.Select(r => r.SpecName).Distinct().OrderBy(s => s));
			}
		}

		public class IndCountCol
		{
			public ColumnFunc<Row, int> Col;

			public IndCountCol()
			{
				Col = new ColumnFunc<Row, int>("IndCount", GetRowValue);
			}

			int GetRowValue(IEnumerable<Row> rows)
			{
				return rows.Select(r => r.IndCount).Sum();
			}
		}


		public abstract class Column<RT, PT> : PropertyDescriptor
		{
			//Type _propType;
	//		Func<object?, object> _getVal;

			public Column(string propName)//, Type propType)//, Func<object?, object> getVal)
				: base(propName, null)
			{
				//_propType = propType;
//				_getVal = getVal;
			}

			public override object? GetValue(object? component)
			{
	//			if (component is RT r)
//					return GetRowValue(r);
				if (component is IEnumerable<RT> rows)
					return GetRowValue(rows);
				else
					throw new Exception("unk type");
			}

			protected abstract PT GetRowValue(IEnumerable<RT> rows);

			//private object GetRowValue(RT r)
			//{
			//	throw new NotImplementedException();
			//}

			//public abstract object GetRowValue()

			public override Type PropertyType => typeof(PT);

			public override void ResetValue(object component)
			{
				// Not relevant.
			}

			public override void SetValue(object? component, object? value) => throw new NotImplementedException();

			public override bool ShouldSerializeValue(object component) => true;
			public override bool CanResetValue(object component) => false;

			public override Type ComponentType => typeof(RT);
			public override bool IsReadOnly => true;
		}


		public class ColumnFunc<TRow, TProp> : PropertyDescriptor
		{
			//Type _propType;
			Func<IEnumerable<TRow>, TProp> _getVal;

			public ColumnFunc(string propName, Func<IEnumerable<TRow>, TProp> getVal)
				: base(propName, null)
			{
				//_propType = propType;
				_getVal = getVal;
			}

			public override object? GetValue(object? component)
			{
				//			if (component is RT r)
				//					return GetRowValue(r);
				if (component is IEnumerable<TRow> rows)
					return _getVal(rows);//: GetRowValue(rows);
				else
					throw new Exception("wrong type");
			}

			public override Type PropertyType => typeof(TProp);

			public override void ResetValue(object component)
			{
				// Not relevant.
			}

			public override void SetValue(object? component, object? value) => throw new NotImplementedException();

			public override bool ShouldSerializeValue(object component) => true;
			public override bool CanResetValue(object component) => false;

			public override Type ComponentType => typeof(IEnumerable<TRow>);
			public override bool IsReadOnly => true;
		}


		class Rows<T> : List<T>, ITypedList
		{
			PropertyDescriptorCollection _pdc;

			public Rows(PropertyDescriptorCollection pdc)
            {
				_pdc = pdc;
            }

            public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
			{
				return _pdc;
			}

			public string GetListName(PropertyDescriptor[] listAccessors)
			{
				return "";
			}
		}

		public class Row
		{
			//public int RowId;
			public string SiteName;
			public string UnitName;
			public string SpecName;
			public int IndCount;

			//public Row(int rowId)
   //         {
			//	RowId = rowId;
   //         }
        }
	}



	public class Field
	{
		public IEqualityComparer<object?> Comparer = EqualityComparer<object?>.Default;

		// FIXME: kind of pointless...could simply used passed order
		//public int Index { get; set; }  // 0, 1, 2

		public Type DataType;

		public string FieldName { get; set; }

		public FieldType FieldType { get; set; } // Group, Data, etc.?

		public Sorting Sorting;
		public int SortIndex;

		public int GroupIndex;

		// FUNC to get display text?
		// Compare\equal by value or text?

		// datavalue (groupin)
		// displayvalue
		// sorting value

		// TOTAL value stored here?

		// Should we cache the value for every row?
		internal int idx;
	}

	public class FieldGen<T> : Field
	{
        public FieldGen()
        {
			DataType = typeof(T);
        }
    }

	public enum FieldType
	{
		Measure = 0,
		RowGroup = 1,
		ColGroup = 2,
	}
	public enum Sorting
	{
		None = 0,
		Asc = 1,
		Desc = 2
	}


	class IntersectData
	{
		Dictionary<Field, object?> values = new();

		internal void Add(Field dataField, object? theValue)
		{
			values.Add(dataField, theValue);
		}
	}

	class Group<T> where T : class
	{
		/// <summary>
		/// If true, not a group but all the rows
		/// </summary>
		public bool IsRoot;

		public object? Key; // data or display? this is raw value. the field funcs decide the groupings via funcs.

		//public IEnumerable<Group<T>> Groups;
		
		public IEnumerable<T> Rows;

		public Field Field;

		public Group<T> ParentGroup;

		public Dictionary<Group<T>, object?[]> IntersectData { get; internal set; }

		public object?[] RowData { get; internal set; }
	}

	public class Pivoter<T> where T : class // notnull
	{
		IEnumerable<Field> _fields;
		IEnumerable<T> _list;
		Dictionary<string, PropertyDescriptor> _props;

		//public Pivoter(IEnumerable<Field> fields, IEnumerable<T> list) : this(fields, list, TypeDescriptor.GetProperties(typeof(T)))
		//{

		//}

		public Pivoter(IEnumerable<Field> fields, IEnumerable<T> list, ITypedList typedList) : this(fields, list, typedList.GetItemProperties(null!))
		{
			
		}

		public Pivoter(IEnumerable<Field> fields, IEnumerable<T> list, PropertyDescriptorCollection pdc)
		{
			//			if (list is not IEnumerable<T>)
			//			throw new ArgumentException("list must be IEnumerable<T>");

			//	_list = (IEnumerable<T>)list;
			_list = list;

			_fields = fields;

			// pdc: aggregator\data getter
			_props = pdc.Cast<PropertyDescriptor>().ToDictionary(pd => pd.Name);

			foreach (var field in _fields)
			{
				var prop = _props[field.FieldName];
			}
		}

		public Table GetTable(bool addGrandTotalRow = false)
		{
			int idx = 0;
			foreach (var f in _fields)
				f.idx = idx++;

			var dataFields = GetDataFields().ToArray();

			//var lastGrr = GroupRowsAllAtOnce(GetGroupFields());
			List<Group<T>> lastGroups = GroupRows(GetGroupFields());

			List<object?[]> rows_o = new();

			var colCount = _fields.Count();

			//if (lastGroups != null)
			{
				foreach (var group in lastGroups)
				{
					var row_o = new object?[colCount];

					foreach (Field dataField in dataFields)
					{
						var getter = _props[dataField.FieldName];
						var theValue = getter.GetValue(group.Rows);
						row_o[dataField.idx] = theValue;
					}

					var parent = group;
					do
					{
						row_o[parent.Field.idx] = parent.Key;
						parent = parent.ParentGroup;
					} while (parent != null && !parent.IsRoot);

					rows_o.Add(row_o);
				}

//				lastGroups = null!; // free mem
			}
			//else // no groups, total sum
			//{
			//	var row_o = new object?[colCount];

			//	foreach (Field dataField in dataFields)
			//	{
			//		var getter = _props[dataField.FieldName];
			//		var theValue = getter.GetValue(_list);
			//		row_o[dataField.idx] = theValue;
			//	}

			//	rows_o.Add(row_o);
			//}
			// else...a mode for output 1:1?
			//{
			//	foreach (var l in _list)
			//	{
			//		var r = res.NewRow();

			//		foreach (Field dataField in GetDataFields())
			//		{
			//			var getter = _props[dataField.FieldName];
			//			// TODO: get value from multiple ROWS
			//			var theValue = getter.GetValue(l.Yield());

			//			r[dataField.FieldName] = theValue;
			//		}
			//	}
			//}


			SortRows(ref rows_o);

			Table t = new();
			t.Columns = _fields.Select(f => new TableColumn { 
				Name = f.FieldName, 
				DataType = f.DataType.Name, 
				Sorting = f.Sorting,
				SortIndex = f.SortIndex,
				FieldType = f.FieldType,
				GroupIndex = f.GroupIndex
			}).ToList();
			t.Rows = rows_o;


			// add sum row after sort, always want it last.
			// only add if we have groups, else it will always be only sum, we dont need double.
			// TODO: should we sum group fields too??
			// TODO: should write "Grand total" or "*" into grouped cols?
			if (addGrandTotalRow)
			{
				if (lastGroups.First().IsRoot)
				{
					// its the same...
					t.GrandTotalRow = rows_o.Single();
				}
				else // if (lastGroups != null)
				{
					var row_o = new object?[colCount];

					foreach (var dataField in dataFields)
					{
						var getter = _props[dataField.FieldName];
						var theValue = getter.GetValue(_list);
						row_o[dataField.idx] = theValue;
					}

					t.GrandTotalRow = row_o;
				}
			}

			// Transform to rows
			//if (lastGroups != null)
			{
				var ggg = lastGroups.GroupBy(lg => GetAllColGroupLevels(lg));
				var lol = ggg.ToList();
			}

			return t;
		}


		public void GroupByAnyColumns(List<string[]> rows, List<int> groupByColumnIndexes)
		{
			var grouped = rows.GroupBy(row => new GroupingKey<string>(groupByColumnIndexes.Select(colIdx => row[colIdx]).ToArray()));
		}


		private GroupingKey<object> GetAllColGroupLevels(Group<T> lg)
		{
			// while parent
			Stack<object> st = new();

			var parent = lg;//.ParentGroup;
			do
			{
				st.Push(parent.Key!);

				parent = parent.ParentGroup;
			} while (parent != null && parent.Field.FieldType == FieldType.ColGroup);

			return new GroupingKey<object>(st.ToArray());
		}

		private void SortRows(ref List<object?[]> rows)
		{
			var sortFields = _fields.Where(f => f.Sorting != Sorting.None).OrderBy(f => f.SortIndex);
			if (sortFields.Any())
			{
				IOrderedEnumerable<object?[]> sorter = null!;
				foreach (var sf in sortFields)
				{
					if (sorter == null)
						sorter = sf.Sorting == Sorting.Asc ? rows.OrderBy(r => r[sf.idx]) : rows.OrderByDescending(r => r[sf.idx]);
					else
						sorter = sf.Sorting == Sorting.Asc ? sorter.ThenBy(r => r[sf.idx]) : sorter.ThenByDescending(r => r[sf.idx]);
				}
				rows = sorter.ToList();
			}
		}

		private List<Group<T>> GroupRowsAllAtOnce(IEnumerable<Field> fields)
		{
			var fieldsArr = fields.ToArray();

			var getters = new PropertyDescriptor[fieldsArr.Length];

			for (int i = 0; i < fieldsArr.Length; i++)
			{
				var getter = _props[fieldsArr[i].FieldName];
				getters[i] = getter;
			}

			GroupingKey<object?> CreateKey(T row)
			{
				var oa = new object?[fieldsArr.Length];

				for (int i = 0; i < fieldsArr.Length; i++)
				{
					oa[i] = getters[i].GetValue(row.Yield());
				}

				return new GroupingKey<object?>(oa);
			}

			var grouped = _list.GroupBy(row => CreateKey(row));


			var res = grouped.Select(g => new Group<T>()
			{
				Key = g.Key,
				Rows = g,
				Field = fieldsArr.Last(),
				ParentGroup = null!
			});

			//return grouped.Select(g => new Pivot.Group { 
			//throw new NotImplementedException();
			//new GroupingKey();
			return res.ToList();
		}

		private List<Group<T>> GroupRows(IEnumerable<Field> fields, bool sort = false)
		{
			List<Group<T>> lastGroups = new List<Group<T>>();
			lastGroups.Add(new Group<T> { Rows = _list, IsRoot = true });

			return GroupRows(lastGroups, fields, sort: sort);
		}

		private List<Group<T>> GroupRows(List<Group<T>> lastGroups, IEnumerable<Field> fields, bool freeOriginalLastGroupsMem = true, bool sort = false)
		{
			List<Group<T>> originalLastGroups = lastGroups;

//			List<Group<T>> lastGroups = new List<Group<T>>();
//		lastGroups.Add(new Group<T> { Rows = _list, IsRoot = true });

			foreach (Field gf in fields)
			{
				var getter = _props[gf.FieldName];

				//if (lastGroups == null)
				//{
				//	var subGroups = _list.GroupBy(r => getter.GetValue(r.Yield()), gf.Comparer).Select(g => new Group<T>()
				//	{
				//		Key = g.Key,
				//		Rows = g,
				//		Field = gf,
				//		ParentGroup = null!
				//	});

				//	lastGroups = subGroups.ToList();
				//}
				//else
				{
					var allSubGroups = new List<Group<T>>();

					foreach (var go in lastGroups)
					{
						var subGroups = go.Rows.GroupBy(r => getter.GetValue(r.Yield()), gf.Comparer).Select(g => new Group<T>()
						{
							Key = g.Key,
							Rows = g,
							Field = gf,
							ParentGroup = go
						});

						if (lastGroups == originalLastGroups)
						{
							if (freeOriginalLastGroupsMem)
								go.Rows = null!; // free mem, no longer needed now we have divided rows futher down in sub groups
						}
						else
						{
							go.Rows = null!; // free mem, no longer needed now we have divided rows futher down in sub groups
						}

						if (sort)
							allSubGroups.AddRange(subGroups.OrderBy(sg => sg.Key)); // displayText or value?
						else
							allSubGroups.AddRange(subGroups);
					}

					lastGroups = allSubGroups;
				}
			}

			return lastGroups;
		}

		public DataTable GetDataTable()
		{
			DataTable res = new();

			foreach (var f in _fields)
			{
				res.Columns.Add(f.FieldName, f.DataType);
			}

			var t = GetTable();

			res.BeginLoadData();
			foreach (var oarr in t.Rows)
				res.LoadDataRow(oarr, fAcceptChanges: false /* ?? */);

			if (t.GrandTotalRow != null)
				res.LoadDataRow(t.GrandTotalRow, fAcceptChanges: false /* ?? */);

			res.EndLoadData();

			return res;
		}

		private IEnumerable<Field> GetDataFields()
		{
			return _fields.Where(f => f.FieldType == FieldType.Measure);//.OrderBy(f => f.Index);
		}

		private IEnumerable<Field> GetGroupFields()
		{
			return _fields.Where(f => f.FieldType == FieldType.RowGroup).OrderBy(f => f.GroupIndex)
				.Concat(_fields.Where(f => f.FieldType == FieldType.ColGroup).OrderBy(f => f.GroupIndex));
		}




		public Table GetTableIntersect()
		{


		//	Table t = new();
			//			t.Columns = _fields.Select(f => new TableColumn { Name = f.FieldName, Type = f.Type.Name }).ToList();


			var dataFields = GetDataFields().ToArray();

			List<Group<T>> lastRowGroups = GroupRows(_fields.Where(f => f.FieldType == FieldType.RowGroup).OrderBy(f => f.GroupIndex));

			var colFields = _fields.Where(f => f.FieldType == FieldType.ColGroup).OrderBy(f => f.GroupIndex).ToList();

			List<Group<T>> lastRowAndColGroups = GroupRows(lastRowGroups, colFields);
			//				.Concat(_fields.Where(f => f.Grouping == Grouping.Col).OrderBy(f => f.GroupIndex)));




			foreach (var lastG in lastRowAndColGroups)
			{
				if (lastRowAndColGroups == lastRowGroups)
				{
					// no col grouping
					lastG.RowData = new object?[dataFields.Length];
				}

				for (int f = 0; f < dataFields.Length; f++)
				{
					var dataField = dataFields[f];

					var getter = _props[dataField.FieldName];

					var theValue = getter.GetValue(lastG.Rows);
	
//					if (lastRowG == lastG)
					if (lastRowAndColGroups == lastRowGroups)
					{
						// no col grouping
						lastG.RowData[f] = theValue;
					}
					else
					{
						// has col groups
						Group<T> lastRowG = GetLastRowGroup(lastG);

						lastRowG.IntersectData ??= new();

						if (!lastRowG.IntersectData.TryGetValue(lastG, out var idata))
						{
							idata = new object?[dataFields.Length];
							lastRowG.IntersectData.Add(lastG, idata);
						}

						idata[f] = theValue;

						// theValue is the intersect of lastRowG (eg /site/Site1) and lastG (eg /feedType/type1)
					}

					// MEN...hva tjener vi på grupperingen??????
					// Kunne vi ikke bare looped den siste gruppen og hentet rowValues og colValues fra parents??
					// Da har vi jo intersecten direkte....

					//		row_o[dataField.idx] = theValue;
				}
			}

			// Want to sort the col groups. Multilevel, just like as for regular rows.
			// GroupRows can do the sorting, need an extra bool for that.
			// very slow
			//			List<Group<T>> lastColGroups = GroupRows(_fields.Where(f => f.Grouping == Grouping.Col).OrderBy(f => f.GroupIndex), sort: true);
			//		lastColGroups.ForEach(g => g.Rows = null!); // free mem


			//if (lastColGroups == null)
			//{
			//	t.Columns = _fields.Select(f => new TableColumn { Name = f.FieldName, Type = f.Type.Name }).ToList();

			//	int idx = 0;
			//	foreach (var f in _fields)
			//		f.idx = idx++;
			//}
			//else
			//{
			//	//var tableColsForRowFields = _fields.Where(f => f.Area == Area.Row).Select(f => new TableColumn { Name = f.FieldName, Type = f.Type.Name });
			//	t.Columns = new();

			//	int idx = 0;
			//	foreach (var f in _fields.Where(f => f.Grouping != Grouping.Col))
			//	{
			//		if (f.Grouping == Grouping.None)
			//		{
			//			f.idx = idx; // start_data_idx
			//			idx += lastColGroups.Count;

			//			foreach (var gr in lastColGroups)
			//			{
			//				Stack<TableGroup> tgs = new();

			//				var parent = gr;
			//				do
			//				{
			//					tgs.Push(new TableGroup { Name = parent.Field.FieldName, Type = parent.Field.Type.Name, Value = parent.Key! });
			//					parent = parent.ParentGroup;
			//				} while (parent != null && !parent.IsRoot);

			//				// /fdfd:34/gfgfg:fdfd/gfgfgfggf
			//				var middle = string.Join('/', tgs.Select(tg => $"{tg.Name}:{tg.Value}"));
			//				var combNAme = $"/{middle}/{f.FieldName}";

			//				t.Columns.Add(new TableColumn { Name = combNAme, Type = f.Type.Name, ColumnGroups = tgs.ToArray() });
			//			}
			//		}
			//		else // row
			//		{
			//			f.idx = idx++;
			//			t.Columns.Add(new TableColumn { Name = f.FieldName, Type = f.Type.Name });
			//		}
			//	}



			//}

			var colGrops = lastRowAndColGroups.GroupBy(lg => GetAllColGroupLevels(lg));//
			colGrops = SortColGroups(colGrops, colFields);

			// (colGroups * dataFields) are the number of cols we need, in addition to cols for rowGroups (one col per row group)


			foreach (var colGrop in colGrops)
			{
				// eksempel: fortype/type1

				var gropingKey = colGrop.Key; // Col group fields
				var groups = gropingKey.Groups; // Col group fields

				foreach (var subGroup in colGrop)
				{
					// eksempel: /fortype/type1 intersect subGroup.RowParents, feks /sites/site1
					// subGroup.Rows her er da rader som matcher alle disse

					// subgruppen har radene
					// MEN...skal dem summeres???????

					var rows = subGroup.Rows;

					// gropp hvis man itererer parents her får man først col groups, deretter row gropups.
					// og det er intersectionen mellom 2 dimensjoner

					
					foreach (Field dataField in dataFields)
					{
						var getter = _props[dataField.FieldName];
						var theValue = getter.GetValue(rows);

						// MEN...hva tjener vi på grupperingen??????
						// Kunne vi ikke bare looped den siste gruppen og hentet rowValues og colValues fra parents??
						// Da har vi jo intersecten direkte....

				//		row_o[dataField.idx] = theValue;
					}


				}
			}

			// Sum data

			//Dictionary<(Group<T>, Group<T>), object?> intersectData = new();

			//if (lastRowGroups != null)
			{
				foreach (var rowGroup in lastRowGroups)
				{
					rowGroup.IntersectData ??= new();

					var hsRowGroupRows = rowGroup.Rows.ToHashSet();


					//var parent = rowGroup;
					//do
					//{
					//	row_o[parent.Field.idx] = parent.Key;
					//	parent = parent.ParentGroup;
					//} while (parent != null && !parent.IsRoot);

					//t.Rows.Add(row_o);
				}
			}
			//else // no row groups, total sum
			//{
			//	var row_o = new object?[t.Columns.Count()];

			//	//foreach (Field dataField in dataFields)
			//	{
			//		if (lastColGroups == null)
			//		{
			//			foreach (Field dataField in dataFields)
			//			{
			//				var getter = _props[dataField.FieldName];
			//				var theValue = getter.GetValue(_list);

			//				row_o[dataField.idx] = theValue;
			//			}
			//		}
			//		else // stuggt....finn bedre måte å handle at det ikke er no gruppe...
			//		{
			//			int colgIdx = 0;
			//			foreach (var colGroup in lastColGroups)
			//			{
			//				foreach (Field dataField in dataFields)
			//				{
			//					var getter = _props[dataField.FieldName];
			//					var theValue = getter.GetValue(colGroup.Rows);

			//					row_o[dataField.idx + colgIdx] = theValue;
			//				}
			//				colgIdx++;
			//			}
			//		}
			//	}

			//	t.Rows.Add(row_o);
			//}
			// else...a mode for output 1:1?
			//{
			//	foreach (var l in _list)
			//	{
			//		var r = res.NewRow();

			//		foreach (Field dataField in GetDataFields())
			//		{
			//			var getter = _props[dataField.FieldName];
			//			// TODO: get value from multiple ROWS
			//			var theValue = getter.GetValue(l.Yield());

			//			r[dataField.FieldName] = theValue;
			//		}
			//	}
			//}



		

			//		return t; // rows_o;
			throw new NotImplementedException();
		}

		private IEnumerable<IGrouping<GroupingKey<object>, Group<T>>> SortColGroups(IEnumerable<IGrouping<GroupingKey<object>, Group<T>>> colGrops, List<Field> colFields)
		{
			//.OrderBy(a => a.Key.Groups[0]).ThenBy(a => a.Key.Groups[1]).ToList();

			//var sortFields = _fields.Where(f => f.Grouping == Grouping.Col)
			//	.Where(f => f.Sorting != Sorting.None)
			//	.OrderBy(f => f.SortIndex)
			//	.ToArray();

			if (colFields.Any())
			{
				IOrderedEnumerable<IGrouping<GroupingKey<object>, Group<T>>> sorter = null!;

				for (int sf = 0; sf < colFields.Count; sf++)
				{
					int local_captured_sf = sf;

					if (sorter == null)
						sorter = colFields[sf].Sorting == Sorting.Asc ? 
							colGrops.OrderBy(r => r.Key.Groups[local_captured_sf]) : colGrops.OrderByDescending(r => r.Key.Groups[local_captured_sf]);
					else
						sorter = colFields[sf].Sorting == Sorting.Asc ? 
							sorter.ThenBy(r => r.Key.Groups[local_captured_sf]) : sorter.ThenByDescending(r => r.Key.Groups[local_captured_sf]);
				}
				colGrops = sorter.ToList(); // tolist needed?
			}

			return colGrops;

		}

		private Group<T> GetLastRowGroup(Group<T> lastG)
		{
			// FIXME: handle IsRoot

			var current = lastG;
			while (current.ParentGroup != null && current.Field.FieldType != FieldType.RowGroup)
			{
				current = current.ParentGroup;
			}

			return current;
		}
	}

	public class TableGroup
	{
		public string Name { get; set; }
		public string Type { get; set; }

		public object Value { get; set; }

		//public TableGroup Parent { get; set; }
	}

	public static class CollExt
	{
		public static IEnumerable<T> Yield<T>(this T t)
		{
			// Alternative: return new[] { t }; is somewhat faster, 10 vs 12 seconds on 2 million calls, but maybe more heavy on memory?
			yield return t;
		}
	}

	public class GroupingKey<T> : IEquatable<GroupingKey<T>>
	{
		public T[] Groups { get; init; }
		static EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;

		public GroupingKey(T[] groups)
		{
			Groups = groups;
		}

		public override int GetHashCode()
		{
			var hc = new HashCode();
			foreach (var g in Groups)
				hc.Add(g);
			return hc.ToHashCode();
		}

		public override bool Equals(object? other)
		{
			return Equals(other as GroupingKey<T>);
		}

		public bool Equals(GroupingKey<T>? other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (Groups.Length != other.Groups.Length)
				return false;

			for (int i = 0; i < Groups.Length; i++)
			{
				if (!equalityComparer.Equals(Groups[i], other.Groups[i]))
					return false;
			}

			return true;
		}

		public override string ToString()
		{
			string[] array = new string[Groups.Length];
			for (int i = 0; i < Groups.Length; i++)
				array[i] = $"Group{i} = {Groups[i]}";

			return $"{{ {string.Join(", ", array)} }}";
		}
	}
}