using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace Pivot
{
	public class Tester
	{
		public static void Main()
		{
			var t = new Tester();
			t.Test();
		}

		public void Test()
		{

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

			list.Add(new Row(1) { IndCount = 11, SiteName = "S1", UnitName = "U1", SpecName = "Frog"});
			list.Add(new Row(1) { IndCount = 13, SiteName = "S1", UnitName = "U1", SpecName = "Hog" });
			list.Add(new Row(2) { IndCount = 22, SiteName = "S1", UnitName = "U2", SpecName = "Bird" });
			list.Add(new Row(3) { IndCount = 33, SiteName = "S2", UnitName = "U1", SpecName = "Dog" });
			list.Add(new Row(4) { IndCount = 44, SiteName = "S3", UnitName = "U1", SpecName = "Human" });
			list.Add(new Row(5) { IndCount = 111, SiteName = "S1", UnitName = "U11", SpecName = "Frog" });
			list.Add(new Row(6) { IndCount = 222, SiteName = "S1", UnitName = "U22", SpecName = "Bird" });
			list.Add(new Row(7) { IndCount = 333, SiteName = "S2", UnitName = "U11", SpecName = "Dog" });
			list.Add(new Row(8) { IndCount = 444, SiteName = "S3", UnitName = "U11", SpecName = "Human" });


			var siteF = new FieldGen<string>() { Area = Area.Data, FieldName = "SiteName", Sort = Sort.Asc, SortIndex = 0 };

			

//			var unitF = new FieldGen<string>() { Area = Area.Group, FieldName = "UnitName"  };
			var specF = new FieldGen<string>() { Area = Area.Group, FieldName = "SpeciesName", Sort = Sort.Desc, SortIndex = 1 };
			var indF = new FieldGen<int>() { Area = Area.Data, FieldName = "IndCount" };
			var ff = new Field[] { specF,   indF, siteF };

			var p = new Pivoter<Row>(ff, list);
			var dt = p.GetTable();
			// TODO: dt can be slow? add option to use different construct? and then need different sorting?
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


		public class ColumnFunc<RT, PT> : PropertyDescriptor
		{
			//Type _propType;
			Func<IEnumerable<RT>, PT> _getVal;

			public ColumnFunc(string propName, Func<IEnumerable<RT>, PT> getVal)
				: base(propName, null)
			{
				//_propType = propType;
				_getVal = getVal;
			}

			public override object? GetValue(object? component)
			{
				//			if (component is RT r)
				//					return GetRowValue(r);
				if (component is IEnumerable<RT> rows)
					return _getVal(rows);//: GetRowValue(rows);
				else
					throw new Exception("wrong type");
			}

			//protected abstract PT GetRowValue(IEnumerable<RT> rows);

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

			public override Type ComponentType => typeof(IEnumerable<RT>);
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
			public int RowId;
			public string SiteName;
			public string UnitName;
			public string SpecName;
			public int IndCount;

			public Row(int rowId)
            {
				RowId = rowId;
            }
        }
	}



	public class Field
	{
		public IEqualityComparer<object?> comparer = EqualityComparer<object?>.Default;

		// FIXME: kind of pointless...could simply used passed order
		//public int Index { get; set; }  // 0, 1, 2

		public Type Type;

		public string FieldName { get; set; }

		public Area Area { get; set; } // Group, Data, etc.?

		public Sort Sort; // asc, desc, none
		public int SortIndex;

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
			Type = typeof(T);
        }
    }

	public enum Area
	{
		Group = 1,
		Data = 2
	}
	public enum Sort
	{
		None = 0,
		Asc = 1,
		Desc = 2
	}

	class Group<T> where T : class
	{
		public object? Key; // data or display? this is raw value. the field funcs decide the groupings via funcs.

		public IEnumerable<Group<T>> Groups;
		
		public IEnumerable<T> Rows;

		public Field Field;

		public Group<T> ParentGroup;
	}

	public class Pivoter<T> where T : class // notnull
	{
		IEnumerable<Field> _fields;
		IEnumerable<T> _list;
		Dictionary<string, PropertyDescriptor> _props;

		public Pivoter(IEnumerable<Field> fields, ITypedList list)
		{
			if (list is not IEnumerable<T>)
				throw new ArgumentException("list must be IEnumerable<T>");

			_list = (IEnumerable<T>)list;

			int idx = 0;
			foreach (var f in fields)
				f.idx = idx++;

			_fields = fields;

			var pdc = list.GetItemProperties(null!);
			_props = pdc.Cast<PropertyDescriptor>().ToDictionary(pd => pd.Name);

			foreach (var field in _fields)
			{
				var prop = _props[field.FieldName];
			}
		}

		public DataTable GetTable()
		{
			DataTable res = new();
			foreach (var f in _fields)
			{
				res.Columns.Add(f.FieldName, f.Type);
			}


			List<object?[]> rows_o = new List<object?[]>();

			var dataFields = GetDataFields().ToArray();

			List<Group<T>> lastGroups = null!;

			foreach (Field gf in GetGroupFields())
			{
				var getter = _props[gf.FieldName];

				if (lastGroups == null)
				{
					//var a = new List<ILookup<object?, T>>();
					// TODO: get value from single ROW
					List<Group<T>> rest = _list.GroupBy(r => getter.GetValue(r.Yield()), gf.comparer).Select(g => new Group<T>() { 
						Key = g.Key, 
						Rows = g,
						Field = gf,
						ParentGroup = null! }).ToList();

					lastGroups = rest;
				}
				else
				{
					var b = new List<Group<T>>();

					//var last = groupLevels.Last();

					// TODO: free mem in previous level

					foreach (var go in lastGroups)
					{
						// TODO: get value from single ROW
						Group<T>[] rest2 = go.Rows.GroupBy(r => getter.GetValue(r.Yield()), gf.comparer).Select(g => new Group<T>() {
							Key = g.Key, 
							Rows = g,
							Field = gf,
							ParentGroup = go}).ToArray();

						go.Groups = rest2;

						go.Rows = null!; // free mem, no longer needed now we have divided rows futher down in sub groups

						b.AddRange(rest2);
					}

					//groupLevels.Add(b);
					lastGroups = b;
				}
			}

			// hva hvis vi ikke har noen grupper?
			if (lastGroups != null)//groupLevels.Any())
			{
				//var lastLevel = groupLevels.Last();
				foreach (var group in lastGroups)
				{
					var r = res.NewRow();

					var row_o = new object?[_fields.Count()];

					foreach (Field dataField in dataFields)
					{
						var getter = _props[dataField.FieldName];
						// TODO: get value from multiple ROWS
						var theValue = getter.GetValue(group.Rows);
						r[dataField.FieldName] = theValue;
						row_o[dataField.idx] = theValue;
					}

					//r[group.Field.FieldName] = group.Key;

					//var parent = group.ParentGroup;
					var parent = group;
					//while (parent != null)
					do
					{
						r[parent.Field.FieldName] = parent.Key;
						row_o[parent.Field.idx] = parent.Key;

						parent = parent.ParentGroup;
					} while (parent != null);
					

					res.Rows.Add(r);
					rows_o.Add(row_o);
				}



				
			}
			else // no groups, total sum
			{
				var r = res.NewRow();
				var row_o = new object?[_fields.Count()];

				foreach (Field dataField in dataFields)
				{
					var getter = _props[dataField.FieldName];
					// TODO: get value from multiple ROWS
					var theValue = getter.GetValue(_list);

					r[dataField.FieldName] = theValue;
					row_o[dataField.idx] = theValue;
				}

				res.Rows.Add(r);
				rows_o.Add(row_o);
			}
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



			res.TableName = "dt";

			var sortFields = _fields.Where(f => f.Sort != Sort.None).OrderBy(f => f.SortIndex);
			if (sortFields.Any())
			{
				res.DefaultView.Sort = string.Join(",", sortFields.Select(f => $"[{f.FieldName}] {(f.Sort == Sort.Asc ? "ASC" : "DESC")}"));

				res = res.DefaultView.ToTable();

				IOrderedEnumerable<object?[]> orderr = null!;
				foreach (var sf in sortFields)
				{
					if (orderr == null)
						orderr = sf.Sort == Sort.Asc ? rows_o.OrderBy(r => r[sf.idx]) : rows_o.OrderByDescending(r => r[sf.idx]);
					else
						orderr = sf.Sort == Sort.Asc ? orderr.ThenBy(r => r[sf.idx]) : orderr.ThenByDescending(r => r[sf.idx]);
				}
				var sorted = orderr.ToList();
			}

			// add sum row after sort, always want it last.
			// only add if we have groups, else it will always be only sum, we dont need double.
			// TODO: should we sum group fields too??
			// TODO: should write "Grand total" or "*" into grouped cols?
			bool addGrandTotal = true;
			if (addGrandTotal)// && lastGroups != null)
			{
				var r = res.NewRow();
				var row_o = new object?[_fields.Count()];

				foreach (var dataField in dataFields)
				{
					var getter = _props[dataField.FieldName];
					// TODO: get value from multiple ROWS
					var theValue = getter.GetValue(_list);

					r[dataField.FieldName] = theValue;
					row_o[dataField.idx] = theValue;
				}

				res.Rows.Add(r);
				rows_o.Add(row_o);
			}

			res.WriteXml(@"d:\testdt.xml");

			return res;
		}

		private IEnumerable<Field> GetDataFields()
		{
			return _fields.Where(f => f.Area == Area.Data);//.OrderBy(f => f.Index);
		}

		private IEnumerable<Field> GetGroupFields()
		{
			return _fields.Where(f => f.Area == Area.Group);//.OrderBy(f => f.Index);
		}
	}

	public static class CollExt
	{
		public static IEnumerable<T> Yield<T>(this T t)
		{
			// Alternative: return new[] { t }; is somewhat faster, 10 vs 12 seconds on 2 million calls, but maybe more heavy on memory?
			yield return t;
		}
	}
}