﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Crypto.Core.Common;
using Crypto.Core.Strategies.Signal;

namespace CryptoMarketClient.Strategies.Signal {
    public partial class TripleRsiStrategyConfigControl : StrategySpecificConfigurationControlBase {
        public TripleRsiStrategyConfigControl() {
            InitializeComponent();
            InitializeDefaults();   
        }
        void InitializeDefaults() {
            var intervals = GetAllowedCandleSticksIntervals(Strategy as TickerStrategyBase);
            comboBoxEdit1.EditValue = intervals.FirstOrDefault();
        }
        protected override void OnStrategyChanged() {
            //TODO check that ticker correctly initiallized on editing.
            List<TickerNameInfo> tickerNameList = Exchange.GetTickersNameInfo();
            if(tickerNameList == null || tickerNameList.Count == 0) {
                XtraMessageBox.Show("Tickers list not initialized. Please close editing form (do not press OK button) and then restart application.");
                return;
            }
            this.tickerNameInfoBindingSource.DataSource = tickerNameList;
            TickerStrategyBase ts = (TickerStrategyBase)Strategy;
            if(ts.TickerInfo != null)
                ts.TickerInfo = tickerNameList.FirstOrDefault(t => t.Ticker == ts.TickerInfo.Ticker);
            this.signalNotificationStrategyBindingSource.DataSource = Strategy;
            string faultExchanges = GetFaultExchanges();

            TickerStrategyBase s = Strategy as TickerStrategyBase;
            if(s?.TickerInfo != null) {
                this.candleStickIntervalInfoBindingSource.DataSource = Exchange.Get(s.TickerInfo.Exchange).AllowedCandleStickIntervals;
                this.comboBoxEdit1.EditValue = GetAllowedCandleSticksIntervals(s).FirstOrDefault(i => (int) i.Interval.TotalMinutes == s.CandleStickIntervalMin);
            }
            if(!string.IsNullOrEmpty(faultExchanges))
                XtraMessageBox.Show("Warning: failed load tickers for the following exchanges: " + faultExchanges);
        }
        void TickerInfoTextEdit_EditValueChanged(object sender, EventArgs e) {
            TickerNameInfo info = this.TickerInfoTextEdit.EditValue as TickerNameInfo;
            ((TickerStrategyBase)Strategy).TickerInfo = info;
            if(info == null) {
                this.candleStickIntervalInfoBindingSource.DataSource = typeof(CandleStickIntervalInfo);
                return;
            }
            Exchange ee = Exchange.Get(info.Exchange);
            this.candleStickIntervalInfoBindingSource.DataSource = ee.AllowedCandleStickIntervals;
        }

        void comboBoxEdit1_EditValueChanged(object sender, EventArgs e) {
            TripleRsiIndicatorStrategy s = (TripleRsiIndicatorStrategy)Strategy;
            CandleStickIntervalInfo info = (CandleStickIntervalInfo)this.comboBoxEdit1.EditValue;
            if(info != null)
                s.CandleStickIntervalMin = (int)info.Interval.TotalMinutes;
            else
                s.CandleStickIntervalMin = (int)TimeSpan.FromHours(4).TotalMinutes;
        }
    }
}
