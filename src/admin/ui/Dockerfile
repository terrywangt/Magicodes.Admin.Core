FROM nginx

COPY nginx.conf /etc/nginx/nginx.conf
COPY . /usr/share/nginx/html
#如果没有证书或者不启用SSL请注释以下代码
COPY xin-lai.com.key /etc/nginx/xin-lai.com.key
COPY xin-lai.com_bundle.crt /etc/nginx/xin-lai.com_bundle.crt
