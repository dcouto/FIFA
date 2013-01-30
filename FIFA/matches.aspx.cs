using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FIFA.COM.DataContext;

namespace FIFA
{
	public partial class matches : System.Web.UI.Page
	{
		private FIFADataContext _dbContext;
		protected FIFADataContext DbContext
		{
			get
			{
				if (_dbContext == null)
				{
					_dbContext = new FIFADataContext();
				}
				return _dbContext;
			}
		}

		protected void Page_Load(object sender, EventArgs e) {
		}

		protected void trgMatches_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e) {
			trgMatches.DataSource = from m in DbContext.Matches
									select m;
		}
	}
}