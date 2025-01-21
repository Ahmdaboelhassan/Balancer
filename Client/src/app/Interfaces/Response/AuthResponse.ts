export interface AuthResponse {
  token: string;
  expireOn: Date;
  isAuth: boolean;
  userName: string;
  message: string;
}
