import os
import sys
import certifi
import urllib3
import numpy as np
import pandas as pd
import datetime as dt


def FileDownload(http, label, filepath):
    req = http.request('GET', file_ulrs[label], preload_content=False)
    with open(filepath + file_names[label], 'wb') as out:
        for chunk in req.stream(chunk_size):
            out.write(chunk)
    req.release_conn()

def GetValue(filePath, filename, position):
    """
    Returns date in the pandas.Timestamp format
    and the value from the file based on the position argument
    """
    try:
        data = pd.read_csv(filePath+filename, 
                           index_col=0, 
                           parse_dates=True, 
                           converters={1: lambda x: np.nan if x=='.' else np.float64(x)})
    except:
        print('cannot read file')
        raise
    data.dropna(axis=0)

    if position == 'LAST':
        index = -1
    elif position == 'YTD':
        if filename == 'LOANSYTD.csv':
            index = -13
        if filename == 'TIGHTENING.csv':
            index = -5
    else:
        raise ValueError('position parameter not valid')
    
    try:
        value = data.iloc[index,0]
        date = data.index[index]
    except IndexError:
        print('IndexError: index out of bounds')
        print('filename: '+filename+', index: '+str(index))
    
    return date, value

class Indicator:
    
    def _Equity(self, filePath):
        k = 0.6565037324782718
        djia_date, djia = GetValue(filePath, file_names['DJIA'], 'LAST')
        mzm_date, mzm = GetValue(filePath, file_names['MZM'], 'LAST')
        return k * djia / mzm

    def _YieldSpread(self, filePath):
        yield_pread_date, yield_spread = GetValue(filePath, file_names['YIELDSPREAD'], 'LAST')
        return yield_spread

    def _RiskLevel(self, filePath):
        risk_date, risk = GetValue(filePath, file_names['VIX'], 'LAST')
        return risk

    def _MoneyThermometer(self, filePath):
        money_therm_date, money_therm = GetValue(filePath, file_names['MZMYTD'], 'LAST')
        return money_therm

    def _Profitability(self, filePath):
        k = 5.1317044568438765
        profits_date, profits = GetValue(filePath, file_names['PROFITS'], 'LAST')
        mzm_date, mzm = GetValue(filePath, file_names['MZM'], 'LAST')
        return k * profits / mzm

    def _BankMeter(self, filePath):
        k = 1.628044909602855
        loans_date, loans = GetValue(filePath, file_names['LOANSYTD'], 'LAST')
        tight_date, tight = GetValue(filePath, file_names['TIGHTENING'], 'LAST')
        loans_ytd_date, loans_ytd = GetValue(filePath, file_names['LOANSYTD'], 'YTD')
        tight_ytd_date, tight_ytd = GetValue(filePath, file_names['TIGHTENING'], 'YTD')
        return k * loans + tight, k * loans_ytd + tight_ytd

    def Calc(self, filePath):
        """
        Saves the indicator values to a file
        """
        lines = []
        lines.append(str(self._Equity(filePath)))
        lines.append(str(self._YieldSpread(filePath)))
        lines.append(str(self._RiskLevel(filePath)))
        lines.append(str(self._MoneyThermometer(filePath)))
        lines.append(str(self._Profitability(filePath)))
        bank, bank_ytd = self._BankMeter(filePath)
        lines.append(str(bank))
        lines.append(str(bank_ytd))
        lines = '\n'.join(lines)
        with open(filePath + 'Indicators.csv','w') as out:
            out.write(lines)


if __name__ == '__main__':
    path = sys.argv[1]
    # Start, End dates
    sdate = str(dt.date.today()-dt.timedelta(days=730))
    edate = str(dt.date.today())

    # Constants
    chunk_size = 32
    indicators = ['Equity',
                'YieldSpread',
                'RiskLevel',
                'MoneyThermometer',
                'Profitability',
                'BankMeter']
    labels = ['DJIA',
            'MZM',
            'VIX',
            'LOANSYTD',
            'TIGHTENING',
            'YIELDSPREAD',
            'PROFITS',
            'MZMYTD']
    file_names = {labels[0]: 'DJIA.csv',
                labels[1]: 'MZM.csv',
                labels[2]: 'VIX.csv',
                labels[3]: 'LOANSYTD.csv',
                labels[4]: 'TIGHTENING.csv',
                labels[5]: 'YIELDSPREAD.csv',
                labels[6]: 'PROFITS.csv',
                labels[7]: 'MZMYTD.csv'}
    file_ulrs = {labels[0]: 'https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd='+sdate+'&coed='+edate+'&height=450&stacking=&range=Custom&mode=fred&id=DJIA&transformation=lin&nd=2008-03-28&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168',
                 labels[1]: 'https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd='+sdate+'&coed='+edate+'&height=450&stacking=&range=&mode=fred&id=MZMSL&transformation=lin&nd=1959-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168',
                 labels[2]: 'https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd='+sdate+'&coed='+edate+'&height=450&stacking=&range=Custom&mode=fred&id=VIXCLS&transformation=lin&nd=1990-01-02&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily%2C+Close&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168',
                 labels[3]: 'https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd='+sdate+'&coed='+edate+'&height=450&stacking=&range=Custom&mode=fred&id=BUSLOANS&transformation=pc1&nd=1947-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168',
                 labels[4]: 'https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd='+sdate+'&coed='+edate+'&height=450&stacking=&range=&mode=fred&id=DRTSCILM&transformation=lin&nd=1990-04-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Quarterly%2C+End+of+Period&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168',
                 labels[5]: 'https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd='+sdate+'&coed='+edate+'&height=450&stacking=&range=Custom&mode=fred&id=T10Y2Y&transformation=lin&nd=1976-06-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168',
                 labels[6]: 'https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd='+sdate+'&coed='+edate+'&height=450&stacking=&range=Custom&mode=fred&id=A053RC1Q027SBEA&transformation=lin&nd=1947-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Quarterly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168',
                 labels[7]: 'https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd='+sdate+'&coed='+edate+'&height=450&stacking=&range=Custom&mode=fred&id=MZMSL&transformation=pc1&nd=1959-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168'}

    file_list = os.listdir(path)
    file_list = [f for f in file_list if f.endswith('.csv')]

    http = urllib3.PoolManager(cert_reqs='CERT_REQUIRED',ca_certs=certifi.where())

    # Checking if all files are donwloaded and up to date
    for item in file_names:
        if file_names[item] in file_list:
            if (dt.datetime.now() - dt.datetime.fromtimestamp(os.path.getmtime(path+file_names[item]))).days > 1:
                FileDownload(http, item, path)
        else:
            FileDownload(http, item, path)

    # Data Processing
    Ind = Indicator()
    Ind.Calc(path)
