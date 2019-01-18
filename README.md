# Oracle_crud
基于C#.net的对oracle的增删改查

<p>用了存储过程、序列、触发器</p>

<p>登陆界面: </p>

<img src="https://img-blog.csdnimg.cn/20190118231009929.png" />
<br>
<p>例子：</p>
<pre><b>//触发器删除学生、课程
create or replace trigger tri_delstu
before delete on stu_inf
 for each row
begin
  delete from stu_course where sid=:old.sid;
end tri_delstu;

create or replace trigger tri_delcourse
before delete on course_inf
 for each row
begin
  delete from stu_course where cid=:old.cid;
end tri_delcourse;</b></pre>
