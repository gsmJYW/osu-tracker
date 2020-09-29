# osu-tracker
지정한 플레이어가 상위 100개 pp 기록을 갱신할 때마다 알려주는 디스코드 봇입니다.

## [DB 생성문]
```sql
CREATE DATABASE `osutracker` /*!40100 DEFAULT CHARACTER SET utf8 */;

CREATE TABLE `targets` (
  `user_id` int(11) NOT NULL,
  `guild_id` char(18) NOT NULL,
  `channel_id` char(18) NOT NULL,
  PRIMARY KEY (`user_id`,`guild_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `pphistories` (
  `user_id` int(11) NOT NULL,
  `previous_pp_sum` double NOT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```
