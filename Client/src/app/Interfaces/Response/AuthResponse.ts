export interface AuthResponse {
  token: string;
  expireOn: Date;
  refreshToken: string;
  refreshTokenExpireOn: Date;
  isAuth: boolean;
  userName: string;
  message: string;
}
