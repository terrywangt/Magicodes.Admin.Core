export interface ListResponse<T>{
  totalCount?: number;
  items?: Array<T>;
}

export interface BufferInfo {
  dataCount: number;
  skipCount: number;
  takeCount: number;
  timestamp: number;
  queryable: boolean;
}
export interface NameValueCollection{
    [name:string]:any;
}

export interface ApiResponse<T>{
  result?: T;
  targetUrl?: string;
  success?: boolean;
  error?: ErrorInfo;
  unAuthorizedRequest?: boolean;
  __abp?: boolean;
}

export interface ErrorInfo {
  code?: number;
  message?: string;
  details?: string;
  validationErrors?: validationError[];
}
export interface validationError {
  message?: string;
  members?: string[];
}