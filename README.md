# osu!tracker
지정한 플레이어가 상위 100개 pp 기록을 갱신할 때마다 알려주는 디스코드 봇입니다.

## 환경 설정
프로그램 사용 시 다음 매개변수를 넣어 실행하세요:

[명령어 접두사] [osu!API v1 키] [디스코드 봇 토큰] [MySQL 서버 주소] [MySQL 포트 번호] [MySQL DB 이름] [MySQL 유저 id] [MySQL 비밀번호]

## DB 생성문
```sql
CREATE DATABASE `osutracker` /*!40100 DEFAULT CHARACTER SET utf8 */;

CREATE TABLE `osutracker`.`users` (
  `discord_id` char(18) NOT NULL,
  `user_id` int NOT NULL,
  PRIMARY KEY (`discord_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `osutracker`.`targets` (
  `user_id` int(11) NOT NULL,
  `guild_id` char(18) NOT NULL,
  PRIMARY KEY (`user_id`,`guild_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `osutracker`.`pphistories` (
  `user_id` int(11) NOT NULL,
  `pp_sum` double NOT NULL,
  `pp_raw` double NOT NULL,
  `pp_rank` int(11) NOT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```
