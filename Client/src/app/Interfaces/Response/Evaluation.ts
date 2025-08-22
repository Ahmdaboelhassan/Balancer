export interface Evaluation {
  id: number;
  from: string;
  to: string;
  name: string | null;
  profit: number;
  profitPercentage?: number;
  income: number;
  note: string;
  lastUpdatedAt?: string;
  createdAt?: string;
  evaluationDetails: EvaluationDetail[];
}

export interface EvaluationDetail {
  id: number;
  accountId: number;
  amount: number;
  percentage: number;
}
