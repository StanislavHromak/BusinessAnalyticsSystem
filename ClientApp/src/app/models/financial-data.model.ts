export interface FinancialData {
  id: number;
  date: string;
  fixedCosts: number;
  variableCostPerUnit: number;
  pricePerUnit: number;
  unitsSold: number;
  investment: number;
  revenue: number;
  totalCosts: number;
  profit: number;
  roi: number;
  ros: number;
}

export interface AnalysisReport {
  labels: string[];
  revenues: number[];
  totalCosts: number[];
  profits: number[];
  rois: number[];
  ross: number[];
}

