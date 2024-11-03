# Take Home Assignment

I recommend using a markdown viewer, to display the Hebrew correctly.

### Instructions (redundant but the "right thing to do")

Clone repo and then in the project's root directory, run `dotnet run` and look at output.

<div dir="rtl">

### נעבור לעברית

אם ראית את המייל ששלחתי לך ... אז הייתי בכיוון אבל רק באופן כללי (בשכונה אבל לא ברחוב הנכון)
<list>

<li>
יש לי מספר מימושים פשוטים של ממשקים שהגדרת, ממש פשוטים בקובץ Simple.cs. מצטער על זה שלא שמתי ניימספייס מיוחד ובקבצים נפרדים.
</li>
<li>
המחלקה העיקרית הוא Service.cs הוא גם מכיל את DI container וגם נקודות כניסה.   
    <li> ד״א השתמשתי בNinject בגלל שזה אפשר לי לאתחל בקוד באופן פשוט בלי קובץ קונפיגורציה (ייתכן שזה לא ייחודי לninject, אבל עשיתי חיפוש וזה היה אחד התכונות שדובר עליו אז הפסקתי לחפש אחרים)_ 
    </li>
</li>
</list>

### כמה מלים על הדיזיין

ראה [drawing.io](https://drive.google.com/file/d/1vvy3gxZ0zHUxJMoWhLyBRfZFMm_GgCoc/view?usp=sharing)

הצרכן לא צריך להכיר את מקור המידע ואת הפרוטוקול של המקור מידע.

למה להשתמש rx? כדי לקבל כביכול event/callback על כל עדכון של דאטה
. לכאורה rx היה גם מאפשר עוד דברים באופן ״שקוף״ כמו ordering, observability, ??

### הגורילה החדר  (ChatGPT)

אז כן, השתמשתי בChatGPT.

<list>
<li>
אני עשיתי את הדיזיין, הגדרתי את כל המחלקות. (הדיזיין הוא באשמתי)
</li>
 <li>
  אם תיקח את ממשקים שהגדרת ותיתן אותם לChatGPT המימושים שהוא יוציא רחוקים הדרישות.
 </li>
 <li>
  בעיקר נתתי לChatGPT לממש מתודות של הממשקים, בחלק מהמקרים זה יצא זבל אבל גם הזבל עזר לכוון אותי. אם לא נגעתי במימוש, אז עברתי כל שורה ואני יכול להסביר כל מה שכתוב.
</li>
<li>
 עוד סיבה שהשתמשתי בChatGPT: עשיתי כמה תרגילים ב-rx לפני 5 שנים והבנתי את העיקרון אבל עדיין לא הרגשתי לגמרי נוח ברמת הקוד, בעיקר איך תופרים את הכל ביחד.
</li>
</list>

### סוף דברים

ראית את זה? [rti pub-sub](https://community.rti.com/static/documentation/connext-dds/current/doc/api/connext_dds/api_csharp/classRti_1_1RequestReply_1_1Replier.html)
נראה די כבד

</div>

חזי בן-מיכאל
