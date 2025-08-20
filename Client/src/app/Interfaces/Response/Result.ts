export interface Result<T> {
  isSucceed: boolean;
  message?: string;
  data?: T;
}
